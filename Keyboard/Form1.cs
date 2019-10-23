using log4net.Config;
using NeatMusic;
using Sanford.Multimedia.Midi;
using SharpNeat.Core;
using SharpNeat.Decoders.Neat;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Keyboard
{
    public partial class Form1 : Form
    {
        OutputStream stream;
        MidiInternalClock clock;

        NeatPlayer rhythm1;
        NeatPlayer rhythm2;
        NeatPlayer pitch1;
        NeatPlayer pitch2;

        SmoothedZSmoothening smoothening1;
        SmoothedZSmoothening smoothening2;
        // parameters for smoothening algorithm
        int LAG = 5;
        double THRESHOLD = 3.5;
        double INFLUENCE = 0.5;

        List<Keys> keysDown = new List<Keys>(); 

        bool modulesOn = false;
        bool noteOn = false;
        double ticksCounter = 0;
        double decayForNote = 0;
        Queue<double> decays = new Queue<double>();

        int currentPitch = 0;
        double currentDurationValue = 0; // this goes to 1 when key is pressed, declines as time passes and goes to 0 when key is lifted
        // WARN this is not consistent with the way the NNs are evolved as we cannot guess how long the key is going to stay pressed
        int module1Pitch = 0;
        double module1Duration = 0;
        int module2Pitch = 0;
        double module2Duration = 0;

        // structure for saving the player's input to send to the evolutionary algorhithm 
        int MAX_AMOUNT = 10;
        Queue<int> recordedPitches = new Queue<int>();
        Queue<double> recordedDurations = new Queue<double>();

        // change fitness thread amount of seconds to change
        int SECONDS_TO_CHANGE = 10;

        // threads
        NeatMusicEvoThread evoObject;
        Thread evoThread;
        ChangeFitnessThread timerObject;
        Thread timerThread;

        public Form1()
        {
            InitializeComponent();
            stream = new OutputStream(0);
            stream.StartPlaying();
            clock = new MidiInternalClock();
            clock.Tick += new EventHandler(calculateOutputs);
            clock.Tempo = 1000000; // 60 bpm

            initializeNetworks();

            // start evolutionary algorithm thread
            UpdateMessage("Starting evolutionary thread.");
            evoObject = new NeatMusicEvoThread();
            evoThread = new Thread(evoObject.doWork);
            evoThread.Start(); // WARN reenable this two lines to activate the evolution thread
            while (!evoThread.IsAlive);

            // start timer thread to change target phrase for fitness
            UpdateMessage("Starting timer for changing fitness target; set to " + SECONDS_TO_CHANGE + "");
            timerObject = new ChangeFitnessThread(SECONDS_TO_CHANGE, this);
            timerThread = new Thread(timerObject.doWork);
            timerThread.Start();
            while (!timerThread.IsAlive);
        }

        private void initializeNetworks()
        {
            //UpdateMessage("Loading neural networks.");
            // initialize modules
            const string CHAMPION_FILE = @"host_parasite_champion_to_load.xml";
            List<NeatGenome> anns;

            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.properties"));

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            //load genomes
            // Save the best genome to file
            XmlReaderSettings xwSettings = new XmlReaderSettings();
            using (XmlReader xw = XmlReader.Create(CHAMPION_FILE, xwSettings))
            {
                anns = NeatGenomeXmlIO.ReadCompleteGenomeList(xw, false);
            }

            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = new NeatGenomeDecoder(new SharpNeat.Decoders.NetworkActivationScheme(2));

            rhythm1 = new NeatPlayer(genomeDecoder.Decode(anns[0]));
            rhythm2 = new NeatPlayer(genomeDecoder.Decode(anns[1]));
            pitch1 = new NeatPlayer(genomeDecoder.Decode(anns[2]));
            pitch2 = new NeatPlayer(genomeDecoder.Decode(anns[3]));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            // properly handle closing the threads
            UpdateMessage("Stopping evolutionary thread...");
            evoObject.RequestStop();
            timerObject.RequestStop();
            evoThread.Join();
            UpdateMessage("Stopping timer thread...");
            timerThread.Join();

            Application.Exit();
        }

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!keysDown.Contains(e.KeyCode))
            {
                keysDown.Add(e.KeyCode);
                //Console.WriteLine(e.KeyCode);
                int note = keyToMidi(e.KeyCode);
                startNote(note, 0);

                //currentDurationValue = 1; // removed this for now to test automatically setting these based on values in the delay queue
                currentPitch = note % 12;

                // start counting ticks used by the note
                if (modulesOn)
                {
                    noteOn = true;
                    ticksCounter = 0;
                }
            }
        }

        private void Form1_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            keysDown.Remove(e.KeyCode);
            //Console.WriteLine(e.KeyCode);
            int note = keyToMidi(e.KeyCode);
            endNote(note, 0);

            //currentDurationValue = 0; // this is not necessary anymore as the decay is based on the previous note
            // when the note is released we calculate the decay to create the inputs to the rhythm ANNs
            if (modulesOn)
            {
                noteOn = false;
                decays.Enqueue(1d / ticksCounter);

                // when note ends add to the recording queue
                if (ticksCounter != 0)
                {
                    recordedPitches.Enqueue(note % 12);
                    recordedDurations.Enqueue(MusicEnvironment.ticksToDuration(ticksCounter));

                    // only hold a specified amount of notes in the recording
                    while (recordedDurations.Count > MAX_AMOUNT)
                        recordedDurations.Dequeue();
                    while (recordedPitches.Count > MAX_AMOUNT)
                        recordedPitches.Dequeue();

                    UpdatePhraseLabels();
                }
            }
        }

        private void startNote(int note, int channel)
        {

            ChannelMessageBuilder builder = new ChannelMessageBuilder();

            builder.Command = ChannelCommand.NoteOn;
            builder.MidiChannel = channel;
            builder.Data1 = note;
            builder.Data2 = 127;
            builder.Build();

            stream.Send(builder.Result);

        }

        private void endNote(int note, int channel)
        {

            ChannelMessageBuilder builder = new ChannelMessageBuilder();

            builder.Command = ChannelCommand.NoteOff;
            builder.MidiChannel = channel;
            builder.Data1 = note;
            builder.Data2 = 0;
            builder.Build();

            stream.Send(builder.Result);

        }

        private int keyToMidi(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Q:
                    return 60;
                case Keys.D2:
                    return 61;
                case Keys.W:
                    return 62;
                case Keys.D3:
                    return 63;
                case Keys.E:
                    return 64;
                case Keys.R:
                    return 65;
                case Keys.D5:
                    return 66;
                case Keys.T:
                    return 67;
                case Keys.D6:
                    return 68;
                case Keys.Y:
                    return 69;
                case Keys.D7:
                    return 70;
                case Keys.U:
                    return 71;
                default:
                    return 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (modulesOn)
            {
                //turn off modules 

                modulesOn = false;
                UpdateMessage("Modules turned off.");
            }
            else
            {
                // turn on modules
                clock.Start();
                smoothening1 = new SmoothedZSmoothening(LAG, THRESHOLD, INFLUENCE);
                smoothening2 = new SmoothedZSmoothening(LAG, THRESHOLD, INFLUENCE);

                modulesOn = true;
                UpdateMessage("Modules turned on.");
            }
        }

        private void calculateOutputs(object sender, EventArgs e)
        {

            if (noteOn)
            {
                ticksCounter += 1;
            }

            if (currentDurationValue == 0 && decays.Count != 0)
            {
                decayForNote = decays.Dequeue();
                currentDurationValue = 1;
            }

            //decay duration value
            currentDurationValue -= decayForNote; // WARN this decay is calculated from the previous note the human played
            if (currentDurationValue < 0)
                currentDurationValue = 0;

            //Console.WriteLine(ticksCounter + "\t" + currentDurationValue);

            if (modulesOn)
            {
                double rhythm1o = rhythm1.calculateOutput(new double[] { currentDurationValue, 0.5 });
                double rhythm2o = rhythm2.calculateOutput(new double[] { currentDurationValue, rhythm1o });
                int pitch1o = MusicEnvironment.convertToMidiClass(pitch1.calculateOutput(new double[] { currentPitch, 0.5 }));
                int pitch2o = MusicEnvironment.convertToMidiClass(pitch2.calculateOutput(new double[] { currentPitch, pitch1o }));

                //if new rhythm value is higher than previous one start new note end end previous note in case there was one
                //if (module1Duration < rhythm1o && !module1rhythmpeak)
                if (smoothening1.isPeak(rhythm1o) == 1)
                {
                    endNote(module1Pitch, 1);
                    startNote(pitch1o + 72, 1);
                    module1Pitch = pitch1o + 72;
                }
                module1Duration = rhythm1o;

                //if (module2Duration < rhythm2o && !module2rhythmpeak)
                if (smoothening2.isPeak(rhythm1o) == 1)
                {
                    endNote(module2Pitch, 2);
                    startNote(pitch2o + 48, 2);
                }
                module2Duration = rhythm2o;

                //if under threshold consider note over
                if (module1Duration < 0.05)
                {
                    endNote(module1Pitch, 1);
                }
                if (module2Duration < 0.05)
                {
                    endNote(module2Pitch, 2);
                }
            }
        }

        public void setNewFitness()
        {
            // tell the thread to copy the champion file when it is not writing it 
            evoObject.copyChampionFile();

            // set networks to champion of previous fitness
            initializeNetworks();

            // change the fitness target so evolution will take a different direction
            if (recordedDurations.Count == MAX_AMOUNT)
                MusicEnvironment.setTargetPhrase(recordedDurations.ToArray(), recordedPitches.ToArray());

            //UpdateMessage("Changed fitness target.");
        }

        private void UpdatePhraseLabels()
        {
            string durations = "";
            foreach (double d in recordedDurations)
                durations += d.ToString("0.00") + " ";
            labelDurationsInfo.Text = durations;

            string pitches = "";
            foreach (int p in recordedPitches)
                pitches += p.ToString() + " ";
            labelPitchesInfo.Text = pitches;
        }

        private void UpdateMessage(string message)
        {
            labelUpdatesInfo.Text += message + Environment.NewLine;
        }
    }
}
