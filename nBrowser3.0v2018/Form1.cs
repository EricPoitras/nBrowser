using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;
using mshtml;
using System.Text.RegularExpressions;
using System.Timers;

namespace nBrowser3._0v2018
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        // Set public variables to keep track of events
        public int goalset = 0; // No goal has been set; 1 a goal has been set
        public int taskindex = 999; // 999 No task has been set; 0, 1, and 2 all correspond to distinct tasks
        public int task1set = 0;
        public int task2set = 0;
        public int task3set = 0; // Keep track of what tasks have been set - 0 is not set; 1 is set
        public int timeset = 0; // Keep track of time allocation to task - 0 to 100 in value (minutes)
        public int subgoal1set = 0;
        public int subgoal2set = 0;
        public int subgoal3set = 0;
        public int subgoal4set = 0; // Keep track of which sub-goal has been achieved
        public int subgoalscomplete = 0; // Count all subgoals achieved
        public int activityindex = 999;
        public int activity1set = 0;
        public int activity2set = 1;
        public int activity3set = 2; // Keep track of type of activity worked on in notes
        public int captionset = 0; // A caption for the lesson has been written; 1 a caption is written; 0 it is not
        public int descriptionset = 0; // A description for the lesson has been written; 1 a description is written; 0 it is not
        public int subjectindex = 999;
        public int educationindex = 999;
        public int standardindex = 999; // Index value of subject matter, educational level, and standard for lesson activity
        public int urllessonset = 0; // The url address for the lesson is set; 0 is not; 1 is set
        public int nametechset = 0; // The name of the technology is set; 0 is not; 1 is set
        public int sourcetechset = 0; // The url address of the technology is set; 0 is not; 1 is set
        public int affordancetechset = 0; // The affordance of the technology is set; 0 is not; 1 is set
        public string selectedhint = "No Selection"; // Hint selection 
        public int taskpanelview = 1;
        public int notepanelview = 0; // Task and note panel view; 1 is visible; 0 is hidden

        public int timersecsession = 0; // Timer in seconds for the learning session duration
        public int starttime;
        public int endtime;
        public int elapsedtime; // Calculate time duration of events for logging purposes

       

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text; //Get url address from textbox

            webBrowser1.Navigate(url); //Navigate to the url address using the web browser

            //MessageBox.Show("Navigation event");
            // Log navigation event
        }

        public void webcrawl(string url)
        {
            //MessageBox.Show(url); Testing OK

            Uri uri;
            if(!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {

                richTextBox1.Text = "Error HTTP Request Not Completed due to Invalid URI";

            }
            else
            {
                // Safely proceed with executing the request
                
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url); // HTTP Request from server
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse(); // HTTP Response from server
                    StreamReader sr = new StreamReader(resp.GetResponseStream()); // Stream Reader from System.IO to read data
                    string sourceCode = sr.ReadToEnd(); // Read data to end
                    sr.Close(); // Close Stream Reader

                    richTextBox1.Text = sourceCode; // Display source code in web browser code view

                    HtmlWeb hw = new HtmlWeb();
                    HtmlAgilityPack.HtmlDocument doc = hw.Load(url);

                    if (doc.DocumentNode.SelectNodes("//h1[@itemprop='name']") != null)
                    {
                        foreach(HtmlNode node in doc.DocumentNode.SelectNodes("//h1[@itemprop='name']"))
                        {
                            string text = node.InnerText;
                            listBox1.Items.Add(text);
                            string id = node.Id;
                            listBox3.Items.Add(id);
                            string prop = "name";
                            listBox4.Items.Add(prop);
                        }
                    }
                    if (doc.DocumentNode.SelectNodes("//span[@itemprop='learningResourceType']") != null)
                    {
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@itemprop='learningResourceType']"))
                        {
                            string text = node.InnerText;
                            listBox1.Items.Add(text);
                            string id = node.Id;
                            listBox3.Items.Add(id);
                            string prop = "learningResourceType";
                            listBox4.Items.Add(prop);
                        }
                    }
                }
                catch
                {
                    richTextBox1.Text = "Error HTTP Request Not Completed due to Null Response";
                }
            }
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            textBox1.Text = webBrowser1.Url.AbsoluteUri;

            //MessageBox.Show(textBox1.Text);
            //Log url address of web navigation event

            webcrawl(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        public void nodematching(string textstring)
        {
            listBox2.Items.Clear();

            if (listBox1.Items.Count > 0)
            {
                foreach (var listBoxItem in listBox1.Items)
                {
                    string nodestring = listBoxItem.ToString();

                    double distancemetric = ComputeDistance(nodestring, textstring);
                    //MessageBox.Show(Convert.ToString(distancemetric));
                    listBox2.Items.Add(distancemetric.ToString());
                }
            }
            else
            {
                //Catch error
            }
        }

        /// <summary>
        /// Compares the two strings based on letter pair matches
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns>The percentage match from 0.0 to 1.0 where 1.0 is 100%</returns>
        public double ComputeDistance(string str1, string str2)
        {
            //https://pastebin.com/EfcmR3Xx Implements string comparison algorithm based on character pair similarity
            List<string> pairs1 = WordLetterPairs(str1.ToUpper());
            List<string> pairs2 = WordLetterPairs(str2.ToUpper());

            int intersection = 0;
            int union = pairs1.Count + pairs2.Count;

            for (int i = 0; i < pairs1.Count; i++)
            {
                for (int j = 0; j < pairs2.Count; j++)
                {
                    if (pairs1[i] == pairs2[j])
                    {
                        intersection++;
                        pairs2.RemoveAt(j);//Must remove the match to prevent "GGGG" from appearing to match "GG" with 100% success

                        break;
                    }
                }
            }

            return (2.0 * intersection) / union;
        }

        /// <summary>
        /// Gets all letter pairs for each
        /// individual word in the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private List<string> WordLetterPairs(string str)
        {
            List<string> AllPairs = new List<string>();

            // Tokenize the string and put the tokens/words into an array
            string[] Words = Regex.Split(str, @"\s");

            // For each word
            for (int w = 0; w < Words.Length; w++)
            {
                if (!string.IsNullOrEmpty(Words[w]))
                {
                    // Find the pairs of characters
                    String[] PairsInWord = LetterPairs(Words[w]);

                    for (int p = 0; p < PairsInWord.Length; p++)
                    {
                        AllPairs.Add(PairsInWord[p]);
                    }
                }
            }

            return AllPairs;
        }

        /// <summary>
        /// Generates an array containing every
        /// two consecutive letters in the input string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string[] LetterPairs(string str)
        {
            int numPairs = str.Length - 1;

            string[] pairs = new string[numPairs];

            for (int i = 0; i < numPairs; i++)
            {
                pairs[i] = str.Substring(i, 2);
            }

            return pairs;
        }

        private void tableBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Start session timer
            timer1.Enabled = true;

            // TODO: This line of code loads data into the 'databaseDataSet1.Table2' table. You can move, or remove it, as needed.
            this.table2TableAdapter.Fill(this.databaseDataSet1.Table2);
            // TODO: This line of code loads data into the 'databaseDataSet.Table' table. You can move, or remove it, as needed.
            this.tableTableAdapter.Fill(this.databaseDataSet.Table);

            // Load image for pedagogical agent
            this.pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\Amy Animation 1.gif");

            //Collapse panel for toolbar
            splitContainer7.Panel2Collapsed = true;
            splitContainer7.Panel1Collapsed = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IHTMLDocument2 htmlDocument = webBrowser1.Document.DomDocument as IHTMLDocument2;
                IHTMLSelectionObject currentSelection = htmlDocument.selection;
                IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;
                if (currentSelection != null)
                {
                    richTextBox2.Text = range.text;
                }

                nodematching(richTextBox2.Text);
                //MessageBox.Show("Paste event to note taking tool");
                // Log paste event to note taking tool
            }
            catch
            {
                // Catch error
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
            //MessageBox.Show("Backward navigation event");
            // Log backward navigation event
        }

        private void button5_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
            //MessageBox.Show("Forward navigation event");
            // Log forward navigation event
        }

        private void button6_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(textBox1.Text);
            
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void updateDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.tableBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.databaseDataSet);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                webBrowser1.Navigate(textBox1.Text);
                //MessageBox.Show("Navigation event");
                // Log navigation event
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(treeView1.SelectedNode != null)
            {
                string textselectednode = treeView1.SelectedNode.Text;
                selectedhint = textselectednode;
                //MessageBox.Show(selectedhint);
                // Log selected hint event

                switch (textselectednode)
                {
                    case "Mathematics Learning Activity Types":
                        richTextBox3.Text = "Essentially, these mathematics activity types are designed to be catalysts to thoughtful and creative instruction by teachers. We have conceptualized seven genres of activity types for mathematics that are derived from the National Council of Teachers of Mathematics’ (NCTM’s) process standards. To encourage active engagement by all students, these activity types are expressed using active words (verbs) to focus instructional planning on student rather than teacher actions.  Many of these words are drawn directly from the NCTM standards.  Each of the seven genres is presented in a separate table that names the activity types for that genre, briefly defines them, and then provides some example technologies that might be selected by a teacher while undertaking each activity. Please note that the specific software titles referenced in the Possible Technologies columns are meant to be illustrative.\n\n1 Grandgenett, N., Harris, J., & Hofer, M. (2011, February). Mathematics learning activity types. Retrieved from College of William and Mary, School of Education, Learning Activity Types: http://activitytypes.wm.edu/Math.html \n\n2   “Mathematics Learning Activity Types” by Neal Grandgenett, Judi Harris and Mark Hofer is licensed under a Creative Commons Attribution - Noncommercial - No Derivative Works 3.0 United States License.Based on a work at http://activitytypes.wm.edu. ";
                        break;
                    case "Consider":
                        richTextBox3.Text = "When learning mathematics, students are often asked to thoughtfully consider new concepts or information.  This request is a familiar one for the mathematics student, and is just as familiar to the teacher. Yet, although such learning activities can be very important contributors to student understanding, the 'Consider' activity types also often represent some of the lower levels of student engagement, and typically are manifested using a relatively direct presentation of foundational knowledge.";
                        break;
                    case "Attend to a Demonstration":
                        richTextBox3.Text = "Students gain information from a presentation, videoclip, animation, interactive whiteboard or other display media. \n\nDocument camera, Content-Specific interactive tool (e.g., ExploreMath) presentation or video creation software, video clips, videoconferencing.";
                        break;
                    case "Read Text":
                        richTextBox3.Text = "Students extract information from textbooks or other written materials, in either print or digital form. \n\nElectronic textbook, websites (i.e. the Math Forum), informational electronic documents (e.g. .pdfs).";
                        break;
                    case "Discuss":
                        richTextBox3.Text = "Students discuss a concept or process with a teacher, other students, or an external expert.\n\nAsk-an-expert site (e.g., Ask Dr. Math), online discussion group, videoconferencing.";
                        break;
                    case "Recognize a Pattern":
                        richTextBox3.Text = "Students examine a pattern presented to them and attempt to understand the pattern better.\n\nGraphing calculators, virtual manipulative site (e.g., the National Library of Virtual Manipulatives), content-specific interactive tool (e.g., ExploreMath), spreadsheet.";
                        break;
                    case "Investigate a Concept":
                        richTextBox3.Text = "Students explore or investigate a concept (such as fractals), perhaps by use of the Internet or other research-related resources.\n\nContent-Specific interactive tool (e.g., ExploreMath), Web searching, informational databases (e.g., Wikipedia), virtual worlds (e.g., Second Life), simulations.";
                        break;
                    case "Understand or Define a Problem":
                        richTextBox3.Text = "Students strive to understand the context of a stated problem or to define the mathematical characteristics of a problem.\n\nWeb searching, concept mapping software, ill-structured problem media (e.g., CIESE Projects).";
                        break;
                    case "Practice":
                        richTextBox3.Text = "In the learning of mathematics, it is often very important for a student to be able to practice computational techniques or other algorithm-based strategies, in order to automate these skills for later and higher-level mathematical applications. Some educational technologies can provide valuable assistance in helping students to practice and internalize important skills and techniques.  This table provides some examples of how technology can assist in these important student practice efforts.";
                        break;
                    case "Do Computation":
                        richTextBox3.Text = "Students undertake computation-based strategies using numeric or symbolic processing.\n\nScientific calculators, graphing calculators, spreadsheet, Mathematica.";
                        break;
                    case "Do Drill and Practice":
                        richTextBox3.Text = "Students rehearse a mathematical strategy or technique, and perhaps uses computer-aided repetition and feedback in the practice process.\n\nDrill and practice software, online textbook supplement, online homework help websites (e.g., WebMath).";
                        break;
                    case "Solve a Puzzle":
                        richTextBox3.Text = "Students carry out a mathematical strategy or technique within the context of solving an engaging puzzle, which may be facilitated or posed by the technology.\n\nVirtual manipulative, Web-based puzzle (e.g., magic squares), mathematical brainteaser Web site (e.g., CoolMath).";
                        break;
                    case "Interpret":
                        richTextBox3.Text = "In the discipline of mathematics, individual concepts and relationships can be quite abstract, and at times can even represent a bit of a mystery to students.  Often students need to spend some time deducing and explaining these relationships to internalize them.  Educational technologies can be used to help students investigate concepts and relationships more actively, and assist them in interpreting what they observe.  This table displays activity types that can support this thoughtful interpretation process, and provides some examples of the available technologies that can be used to support forming the interpretations.";
                        break;
                    case "Pose a Conjecture":
                        richTextBox3.Text = "The student poses a conjecture, perhaps using dynamic software to display relationships.\n\nDynamic geometry software (e.g., Geometer’s Sketchpad), Content-specific interactive tool (e.g., ExploreMath), e-mail.";
                        break;
                    case "Develop an Argument":
                        richTextBox3.Text = "The student develops a mathematical argument related to why they think that something is true.  Technology may help to form and to display that argument.\n\nConcept mapping software, presentation software, blog, specialized word processing software (e.g., Theorist).";
                        break;
                    case "Categorize":
                        richTextBox3.Text = "The student attempts to examine a concept or relationship in order to categorize it into a set of known categories.\n\nDatabase software, online database, concept mapping software, drawing software.";
                        break;
                    case "Interpret a Representation":
                        richTextBox3.Text = "The student explains the relationships apparent from a mathematical representation (table, formula, chart, diagram, graph, picture, model, animation, etc.).\n\nData visualization software (e.g., Inspire Data), 2D and 3D animation, video clip, Global Positioning Devices (GPS), engineering-related visualization software (e.g., MathCad).";
                        break;
                    case "Estimate":
                        richTextBox3.Text = "The student attempts to approximate some mathematical value by further examining relationships using supportive technologies.\n\nScientific calculator, graphing calculator, spreadsheet, student response system (e.g. “clickers”).";
                        break;
                    case "Interpret a Phenomenon Mathematically":
                        richTextBox3.Text = "Assisted by technology as needed, the student examines a mathematics-related phenomenon (such as velocity, acceleration, the Golden Ratio, gravity, etc.).\n\nDigital camera, video, computer-aided laboratory equipment, engineering-related visualization software, specialized word processing software (e.g., Theorist), robotics, electronics kit.";
                        break;
                    case "Produce":
                        richTextBox3.Text = "When students are actively engaged in the study of mathematics, they can become motivated producers of mathematical works, rather than just passive consumers of prepared materials.  Educational technologies can serve as excellent “partners” in this production process, aiding in the refinement and formalization of a student product, as well as helping the student to share the fruits of their mathematical labors.  The activity types listed below suggest technology-assisted efforts in which students become “producers” of mathematics-related products.";
                        break;
                    case "Do a Demonstration":
                        richTextBox3.Text = "The student makes a demonstration on some topic to show their understanding of a mathematical idea or process.  Technology may assist in the development or presentation of the product.\n\nInteractive whiteboard, video creation software,  document camera, presentation software, podcast, video sharing site.";
                        break;
                    case "Generate Text":
                        richTextBox3.Text = "The student produces a report, annotation, explanation, journal entry or document, to illustrate their understanding.\n\nSpecialized word processing software (e.g., Math Type), collaborative word processing software, blog, online discussion group.";
                        break;
                    case "Describe an Object or Concept Mathematically":
                        richTextBox3.Text = "Assisted by the technology in the description or documentation process, the student produces a mathematical explanation of an object or concept.\n\nLogo graphics, engineering visualization software, concept mapping software, specialized word processing software, Mathematica.";
                        break;
                    case "Produce a Representation":
                        richTextBox3.Text = "Using technology for production assistance if appropriate, the student develops a mathematical representation (table, formula, chart, diagram, graph, picture, model, animation, etc.).\n\nSpreadsheet, virtual manipulatives (e.g., digital geoboard), document camera, concept mapping software, graphing calculator.";
                        break;
                    case "Develop a Problem":
                        richTextBox3.Text = "The student poses a mathematical problem that is illustrative of some mathematical concept, relationship, or investigative question.\n\nWord processing software, online discussion group, Wikipedia, Web searching, e-mail.";
                        break;
                    case "Apply":
                        richTextBox3.Text = "The utility of mathematics in the world can be found in its authentic application.  Educational technologies can be used to help students to apply their mathematical knowledge in the real world, and to link specific mathematical concepts to real world phenomena.  The technologies essentially become students’ assistants in their mathematical work, helping them to link the mathematical concepts being studied to the reality in which they live.";
                        break;
                    case "Choose a Strategy":
                        richTextBox3.Text = "The student reviews or selects a mathematics-related strategy for a particular context or application.\n\nOnline help sites (e.g., WebMath, Math Forum), Inspire Data, dynamic geometry/algebra software (e.g., Geometry Expressions), Mathematica, MathCAD.";
                        break;
                    case "Take a Test":
                        richTextBox3.Text = "The student demonstrates their mathematical knowledge within the context of a testing environment, such as with computer-assisted testing software.\n\nTest-taking software, Blackboard, online survey software, student response system (e.g. “clickers”).";
                        break;
                    case "Apply a Representation":
                        richTextBox3.Text = "The student applies a mathematical representation to a real life situation (table, formula, chart, diagram, graph, picture, model, animation, etc.).\n\nSpreadsheet, robotics, graphing calculator, computer-aided laboratories, virtual manipulatives (e.g., electronic algebra tiles).";
                        break;
                    case "Evaluate":
                        richTextBox3.Text = "When students evaluate the mathematical work of others, or self-evaluate their own mathematical work, they engage in a relatively sophisticated effort to try to understand mathematical concepts and processes.  Educational technologies can become valuable allies in this effort, assisting students in the evaluation process by helping them to undertake concept comparisons, test solutions or conjectures, and/or integrate feedback from other individuals into revisions of their work.  The following table lists some of these evaluation-related activities.";
                        break;
                    case "Compare and Contrast":
                        richTextBox3.Text = "The student compares and contrasts different mathematical strategies or concepts, to see which is more appropriate for a particular situation.\n\nConcept mapping software (e.g., Inspiration), Web searches, Mathematica, MathCad.";
                        break;
                    case "Test a Solution":
                        richTextBox3.Text = "The student systematically tests a solution, and examines whether it makes sense based upon systematic feedback, which might be assisted by technology.\n\nScientific calculator, graphing calculator, spreadsheet, Mathematica, Geometry Expressions.";
                        break;
                    case "Test a Conjecture":
                        richTextBox3.Text = "The student poses a specific conjecture and then examines the feedback of any interactive results to potentially refine the conjecture.\n\nGeometer Sketchpad, content-specific interactive tool (e.g., ExploreMath), statistical packages (e.g., SPSS, Fathom), online calculator, robotics.";
                        break;
                    case "Evaluate Mathematical Work":
                        richTextBox3.Text = "The student evaluates a body of mathematical work, through the use of peer or technology-aided feedback.\n\nOnline discussion group, blog, Mathematica, MathCad, Inspire Data.";
                        break;
                    case "Create":
                        richTextBox3.Text = "When students are involved in some of the highest levels of mathematics learning activities, they are often engaged in very creative and imaginative thinking processes. Albert Einstein once suggested that “imagination is more important than knowledge.”  It is said that this quote represents his strong belief that mathematics is a very inventive, inspired, and imaginative endeavor.  Educational technologies can be used to help students to be creative in their mathematical work, and even to help other students to deepen their learning of the mathematics that they already understand.  The activity types below represent these creative elements and processes in students’ mathematical learning and interaction.";
                        break;
                    case "Teach a Lesson":
                        richTextBox3.Text = "The student develops and delivers a lesson on a particular mathematics concept, strategy, or problem.\n\nDocument camera, presentation software, videoconferencing, video creation software, podcast.";
                        break;
                    case "Create a Plan":
                        richTextBox3.Text = "The student develops a systematic plan to address some mathematical problem or task.\n\nConcept mapping software, collaborative word processing software, MathCad, Mathematica.";
                        break;
                    case "Create a Product":
                        richTextBox3.Text = "The student imaginatively engages in the development of a student project, invention, or artifact, such as a new fractal, a tessellation, or another creative product.\n\nWord processing software, videocamera, animation tools, MathCad, Mathematica, Geometer Sketchpad.";
                        break;
                    case "Create a Process":
                        richTextBox3.Text = "The student creates a mathematical process that others might use, test or replicate, essentially engaging in mathematical creativity.\n\nComputer programming, robotics, Mathematica, MathCad, Inspire Data, video creation software.";
                        break;
                }
            }
        }

        private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
                    }

        private void tableLayoutPanel25_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            splitContainer7.Panel1Collapsed = false;
            splitContainer7.Panel2Collapsed = true;

            taskpanelview = 1;
            notepanelview = 0;

            //MessageBox.Show("Task panel is visible");
            // Log Task panel view event
        }

        private void button8_Click(object sender, EventArgs e)
        {
            splitContainer7.Panel1Collapsed = true;
            splitContainer7.Panel2Collapsed = false;

            notepanelview = 1;
            taskpanelview = 0;

            //Message.Show("Note panel is visible");
            // Log Note panel view event
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object has superior alignment only if both of the following are true: All of the content and performance expectations in the identified standard are completely addressed by the object. The content and performance expectations of the identified standard are the focus of the object.While some objects may cover a range of standards that could potentially be aligned, for a superior alignment the content and performance expectations must not be a peripheral part of the object. 2: An object has strong alignment for either one of two reasons: Minor elements of the standard are not addressed in the object. The content and performance expectations of the standard align to a minor part of the object. 1: An object has limited alignment if a significant part of the content or performance expectations of the identified standard is not addressed in the object, as long as there is fidelity to the part it does cover.For example, an object that aligns to CCSS 2.NBT.2, “Count within 1000; skip - count by 5s, 10s, and 100s,” but only addresses counting numbers to 500, would be considered to have limited alignment.The object aligns very closely with a limited part of the standard. 0: An object has very weak alignment for either one of two reasons: The object does not match the intended standards. The object matches only to minimally important aspects of a standard.These objects will not typically be useful for instruction of core concepts and performances covered by the standard. N / A: This rubric does not apply for an object that has no suggested standards for alignment. For example, the rubric might not be applicable to a set of raw data. ";
            //Log Link 1
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object is rated superior for explanation of subject matter only if all of the following are true: The object provides comprehensive information so effectively that the target audience should be able to understand the subject matter. The object connects important associated concepts within the subject matter.For example, a lesson on multi - digit addition makes connections with place value, rather than simply showing how to add multi - digit numbers.Or a lesson designed to analyze how an author develops ideas across extended text would make connections among the various developmental steps and the various purposes the author has for the text. The object does not need to be augmented with additional explanation or materials. The main ideas of the subject matter addressed in the object are clearly identified for the learner. 2: An object is rated strong for explanation of subject matter if it explains the subject matter in a way that makes skills, procedures, concepts, and / or information understandable.It falls short of superior in that it does not make connections among important associated concepts within the subject matter. For example, a lesson on multi - digit addition may focus on the procedure and fail to connect it with place value. 1: An object is rated limited for explanation of subject matter if it explains the subject matter correctly but in a limited way.This cursory treatment of the content is not sufficiently developed for a first-time learner of the content.The explanations are not thorough and would likely serve as a review for most learners. 0: An object is rated very weak or no value for explanation of subject matter if its explanations are confusing or contain errors. There is little likelihood that this object will contribute to understanding. N / A: This rubric is not applicable (N / A) for an object that is not designed to explain subject matter, for example, a sheet of mathematical formulae or a map.It may be possible to apply the object in some way that aids a learner’s understanding, but that is beyond any obvious or described purpose of the object.";
            //Log Link 2
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object is rated superior for the utility of materials designed to support teaching only if all of the following are true: The object provides materials that are comprehensive and easy to understand and use. The object includes suggestions for ways to use the materials with a variety of learners. These suggestions include materials such as “common error analysis tips” and “precursor skills and knowledge” that go beyond the basic lesson or unit elements. All objects and all components are provided and function as intended and described.For example, the time needed for lesson planning appears accurately estimated, materials lists are complete, and explanations make sense. For larger objects like units, materials facilitate the use of a mix of instructional approaches(direct instruction, group work, investigations, etc.). 2: An object is rated strong for the utility of materials designed to support teaching if it offers materials that are comprehensive and easy to understand and use but falls short of “superior” for either one of two reasons: The object does not include suggestions for ways to use the materials with a variety of learners(e.g., error analysis tips). Some core components(e.g., directions) are underdeveloped in the object. 1: An object is rated limited for the utility of materials designed to support teaching if it includes a useful approach or idea to teach an important topic but falls short of “strong” for either one of two reasons: The object is missing important elements(e.g.directions for some parts of a lesson are not included). Important elements do not function as they are intended to(e.g.directions are unclear or practice exercises are missing or inadequate).Teachers would need to supplement this object to use it effectively. 0: An object is rated very weak or no value for the utility of materials designed to support teaching if it is confusing, contains errors, is missing important elements, or is for some other reason simply not useful, in spite of an intention to be used as a support for teachers in planning or preparation. N / A: This rubric is not applicable (N / A) for an object that is not designed to support teachers in planning and / or presenting subject matter.It may be possible that an educator could find an application for such an object during a lesson, but that would not be the intended use";
            // Log Link 3
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object is rated superior for the quality of its assessments only if all of the following are true: All of the skills and knowledge assessed align clearly to the content and performance expectations intended, as stated or implied in the object. Nothing is assessed that is not included in the scope of intended material unless it is differentiated as extension material. The most important aspects of the expectations are targeted and are given appropriate weight/ attention in the assessment. The assessment modes used in the object, such as selected response, long and short constructed response, or group work require the student to demonstrate proficiency in the intended concept / skill. The level of difficulty is a result of the complexity of the subject-area content and performance expectations and of the degree of cognitive demand, rather than a result of unrelated issues(e.g.overly complex vocabulary used in math word problems). 2: An object is rated strong for the quality of its assessments if it assesses all of the content and performance expectations intended, but the assessment modes used do not consistently offer the student opportunities to demonstrate proficiency in the intended concept / skill. 1: An object is rated limited for the quality of its assessments if it assesses some of the content or performance expectations intended, as stated or implicit in the object, but omits some important content or performance expectations and/or fails to offer the student opportunities to demonstrate proficiency in the intended content/skills. 0: An object is rated very weak or no value for the quality of its assessments if its assessments contain significant errors, do not assess important content/skills, are written in a way that is confusing to students, or are unsound for other reasons. N/A: This rubric is not applicable (N/A) for an object that is not designed to have an assessment component.Even if one might imagine ways an object could be used for assessment purposes, if it is not the intended purpose, not applicable is the appropriate score.";
            // Log Link 4
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object, or interactive component of an object, is rated superior for the quality of its technological interactivity only if all of the following are true: The object is responsive to student input in a way that creates an individualized learning experience. This means the object adapts to the user based on what s / he does, or the object allows the user some flexibility or individual control during the learning experience. The interactive element is purposeful and directly related to learning. The object is well - designed and easy to use, encouraging learner use. The object appears to function flawlessly on the intended platform. 2: An object, or interactive component of an object, is rated strong for the quality of its technological interactivity if it has an interactive feature that is purposeful and directly related to learning, but does not provide an individualized learning experience.Similarly to the superior objects, strong interactive objects must be well designed, easy - to - use, and function flawlessly on the intended platform.Some technological elements may not be directly related to the content but for a strong rating they must not detract from the learning experience.These kinds of interactive elements, including earning points or achieving levels for correct answers, might be designed to increase student motivation and to build content understanding by rewarding or entertaining the learner, and may extend the time the user engages with the content. 1: An object, or interactive component of an object, is rated limited for the quality of its technological interactivity if its interactive element does not relate to the subject matter and may detract from the learning experience.These kinds of interactive elements may slightly increase motivation but do not provide strong support for understanding the subject matter addressed in the object.It is unlikely that this interactive feature will increase understanding or extend the time a user engages with the content. 0: An object, or interactive component of an object, is rated very weak or no value for the quality of its technological interactivity if it has interactive features that are poorly conceived and/ or executed.The interactive features might fail to operate as intended, distract the user, or unnecessarily take up user time. ";
            // Log Link 5
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object is rated superior for the quality of its instructional and practice exercises only if all of the following are true: The object offers more exercises than needed for the average student to facilitate mastery of the targeted skills, as stated or implied in the object.For complex tasks, one or two rich practice exercises may be considered more than enough. The exercises are clearly written and supported by accurate answer keys or scoring guidelines as applicable. There are a variety of exercise types and / or the exercises are available in a variety of formats, as appropriate to the targeted concepts and skills.For more complex practice exercises the formats used provide an opportunity for the learner to integrate a variety of skills. 2: An object is rated strong for the quality of its instructional and practice exercises if it offers only a sufficient number of well - written exercises to facilitate mastery of targeted skills, which are supported by accurate answer keys or scoring guidelines, but there is little variety of exercise types or formats. 1: An object is rated limited for the quality of its instructional and practice exercises if it has some, but too few exercises to facilitate mastery of the targeted skills, is without answer keys, and provides no variation in type or format. 0: An object is rated very weak or no value for the quality of its instructional and practice exercises if the exercises provided do not facilitate mastery of the targeted skills, contain errors, or are unsound for other reasons. N / A: This rubric is not applicable (N / A) to an object that does not include opportunities to practice targeted skills.";
            // Log Link 6
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "3: An object is rated superior for its opportunities for deeper learning only if all of the following are true: At least three of the deeper learning skills from the list identified in this rubric are required in the object. The object offers a range of cognitive demand that is appropriate and supportive of the material. Appropriate scaffolding and direction are provided. 2: An object is rated strong for its opportunities for deeper learning if it includes one or two deeper learning skills identified in this rubric.For example, the object might involve a complex problem that requires abstract reasoning skills to reach a solution. 1: An object is rated limited for its opportunities for deeper learning if it includes one deeper learning skill identified in the rubric but is missing clear guidance on how to tap into the various aspects of deeper learning. For example, an object might include a provision for learners to collaborate, but the process and product are unclear. 0: An object is rated very weak for its opportunities for deeper learning if it appears to be designed to provide some of the deeper learning opportunities identified in this rubric, but it is not useful as it is presented.For example, the object might be based on poorly formulated problems and/or unclear directions, making it unlikely that this lesson or activity will lead to skills like critical thinking, abstract reasoning, constructing arguments, or modeling. N/A: This rubric is not applicable(N/A) to an object that does not appear to be designed to provide the opportunity for deeper learning, even though one might imagine how it could be used to do so.";
            // Log Link 7
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            richTextBox8.Text = "This rubric is used to assure materials are accessible to all students, including students identified as blind, visually impaired or print disabled, and those students who may qualify under the Chafee Amendment to the U.S. 1931 Act to Provide Books to the Adult Blind as Amended.It was developed to assess compliance with U.S.standards and requirements, but could be adapted to accommodate differences in other sets of requirements internationally. Accessibility is critically important for all learners and should be considered in the design of all online materials.Identification of certain characteristics will assist in determining if materials will be fully accessible for all students. Assurance that materials are compliant with the standards, recommendations, and guidelines specified assists educators in the selection and use of accessible versions of materials that can be used with all students, including those with different kinds of challenges and assistive devices. The Assurance of Accessibility Standards Rubric does not ask reviewers to make a judgment on the degree of object quality.Instead, it requests that a determination(yes / no) of characteristics be made that, together with assurance of specific Standards, may determine the degree to which the materials are accessible.Only those who feel qualified to make judgments about an object’s accessibility should use this rubric. ";
            // Log Link 8
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox7_TextChanged(object sender, EventArgs e)
        {
            // Log character entry in goal box
        }

        private void richTextBox7_Validated(object sender, EventArgs e)
        {
            if(richTextBox7.Text.Equals("") || richTextBox7.Text.Equals("Write a brief description of the objective of your lesson."))
            {
                // Log goal setting box loses focus - no edit to text value

            }
            else
            {
                goalset = 1;
                //MessageBox.Show(goalset.ToString());

                // Log value of goal setting edit event
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            taskindex = comboBox1.SelectedIndex;
            //MessageBox.Show(taskindex.ToString());

            if(taskindex == 0)
            {
                task1set = 1; // assess is set
                //MessageBox.Show(task1set.ToString());

                // Log third task is selected - assess student understanding
            }
            else if(taskindex == 1)
            {
                task2set = 1; // demonstrate is set
                //MessageBox.Show(task2set.ToString());
                // Log third task is selected - demonstrate concept
            }
            else if(taskindex == 2)
            {
                task3set = 1; // reflect is set
                //MessageBox.Show(task3set.ToString());
                // Log third task is selected - foster reflection
            }
            else
            {
                // Log unrecognized task index selected
            }
        }

        private void numericUpDown1_Validated(object sender, EventArgs e)
        {
            timeset = (int)numericUpDown1.Value;
            if(timeset == 0)
            {
                //MessageBox.Show(timeset.ToString());
                // Log no time allocated to task
            }
            else
            {
                //MessageBox.Show(timeset.ToString());
                // Log time amount allocated to task
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            subgoal1set = 0;
            subgoal2set = 0;
            subgoal3set = 0;
            subgoal4set = 0;
            //MessageBox.Show("Subgoals are reset");
            // Reset all subgoals

            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                subgoalscomplete = checkedListBox1.CheckedItems.Count;
                //MessageBox.Show(checkedListBox1.CheckedItems.Count.ToString());
                // Count number of subgoals selected
                if (checkedListBox1.CheckedIndices[i] == 0)
                {
                    subgoal1set = 1;
                    //MessageBox.Show("Subgoal 1 is selected");
                    // Log subgoal 1 is selected
                }
                else if(checkedListBox1.CheckedIndices[i] == 1)
                {
                    subgoal2set = 1;
                    //MessageBox.Show("Subgoal 2 is selected");
                    // Log subgoal 2 is selected
                }
                else if (checkedListBox1.CheckedIndices[i] == 2)
                {
                    subgoal3set = 1;
                    //MessageBox.Show("Subgoal 3 is selected");
                    // Log subgoal 3 is selected
                }
                else if (checkedListBox1.CheckedIndices[i] == 3)
                {
                    subgoal4set = 1;
                    //MessageBox.Show("Subgoal 4 is selected");
                    // Log subgoal 4 is selected
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            activityindex = comboBox3.SelectedIndex;
            //MessageBox.Show(activityindex.ToString());

            if (activityindex == 0)
            {
                activity1set = 1; // present is set
                //MessageBox.Show(comboBox3.SelectedText);

                // Log first activity is selected - demonstrate concept
            }
            else if (activityindex == 1)
            {
                activity2set = 1; // assess is set
                //MessageBox.Show(comboBox3.SelectedText);
                // Log second activity is selected - assess student understanding
            }
            else if (activityindex == 2)
            {
                activity3set = 1; // reflect is set
                //MessageBox.Show(comboBox3.SelectedText);
                // Log third activity is selected - foster reflection
            }
            else
            {
                // Log unrecognized activity index selected
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for activity brief caption
        }

        private void textBox4_Validated(object sender, EventArgs e)
        {
            if(textBox4.Text.Equals("") || textBox4.Text.Equals("Add a brief title for the activity"))
            {
                //MessageBox.Show("Text validation event on lesson caption not valid");
                // Log text validation event on lesson caption is not valid
            }
            else
            {
                //MessageBox.Show(textBox4.Text);
                // Log text validation event content for textbox with lesson caption
                captionset = 1;
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for lesson description
        }

        private void richTextBox2_Validated(object sender, EventArgs e)
        {
            if (richTextBox2.Text.Equals("") || richTextBox2.Text.Equals("Write a description of the lesson activity"))
            {
                //MessageBox.Show("Text validation event on lesson description not valid");
                // Log text validation event on lesson description is not valid
            }
            else
            {
                //MessageBox.Show(richTextBox2.Text);
                // Log text validation event content for textbox with lesson description
                descriptionset = 1;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            subjectindex = comboBox5.SelectedIndex;
            //MessageBox.Show(subjectindex.ToString());
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            educationindex = comboBox6.SelectedIndex;
            //MessageBox.Show(educationindex.ToString());
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            standardindex = comboBox7.SelectedIndex;
            //MessageBox.Show(standardindex.ToString());
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for lesson url address
        }

        private void textBox5_Validated(object sender, EventArgs e)
        {
            if (textBox5.Text.Equals("") || textBox5.Text.Equals("Add a URL to the lesson plan"))
            {
                //MessageBox.Show("Text validation event on lesson url not valid");
                // Log text validation event on lesson url is not valid
            }
            else
            {
                //MessageBox.Show(textBox5.Text);
                // Log text validation event content for textbox with lesson url
                urllessonset = 1;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for name of technology
        }

        private void textBox6_Validated(object sender, EventArgs e)
        {
            if (textBox6.Text.Equals("") || textBox6.Text.Equals("Add the name of the technology"))
            {
                //MessageBox.Show("Text validation event on technology name not valid");
                // Log text validation event on name of technology
            }
            else
            {
                //MessageBox.Show(textBox6.Text);
                // Log text validation event content for textbox with name of technology
                nametechset = 1;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for technology source
        }

        private void textBox7_Validated(object sender, EventArgs e)
        {
            if (textBox7.Text.Equals("") || textBox7.Text.Equals("Add a URL to the website"))
            {
                //MessageBox.Show("Text validation event on technology url address not valid");
                // Log text validation event on url address of technology not valid
            }
            else
            {
                //MessageBox.Show(textBox7.Text);
                // Log text validation event content for textbox with url address of technology
                sourcetechset = 1;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for technology affordance
        }

        private void textBox8_Validated(object sender, EventArgs e)
        {
            if (textBox8.Text.Equals("") || textBox8.Text.Equals("Identify the type of affordance (see below)"))
            {
                //MessageBox.Show("Text validation event on technology affordance not valid");
                // Log text validation event on technology affordance not valid
            }
            else
            {
                //MessageBox.Show(textBox8.Text);
                // Log text validation event content for textbox with technology affordance
                affordancetechset = 1;
            }
        }

        private void richTextBox5_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(richTextBox5.Text);
            // Log text validation event for Amy prompt window
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //MessageBox.Show("Help Request");
            // Log help request event
        }

        private void richTextBox6_TextChanged(object sender, EventArgs e)
        {
            // Log character press event for response to Amy
        }

        private void richTextBox6_Validated(object sender, EventArgs e)
        {
            if (textBox6.Text.Equals("") || textBox6.Text.Equals("Write a response to Amy here..."))
            {
                //MessageBox.Show("Text validation event on Amy response not valid");
                // Log text validation event on Amy response not valid
            }
            else
            {
                //MessageBox.Show(textBox6.Text);
                // Log text validation event content for textbox with Amy response
               
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //MessageBox.Show("Submit Amy response");
            // Log button submit event for Amy response
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
            {
                //MessageBox.Show("Resource tab is visible");
                // Log resource tab view event
            }
            else if(tabControl1.SelectedIndex == 1)
            {
                //MessageBox.Show("Community tab is visible");
                // Log community tab view event
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                //MessageBox.Show("Rating tab is visible");
                // Log rating tab view event
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox2.SelectedIndex.ToString());
            // Log index change 1
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox4.SelectedIndex.ToString());
            // Log index change 2
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox8.SelectedIndex.ToString());
            // Log index change 3
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox9.SelectedIndex.ToString());
            // Log index change 4
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox10.SelectedIndex.ToString());
            // Log index change 5
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox11.SelectedIndex.ToString());
            // Log index change 6
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox12.SelectedIndex.ToString());
            // Log index change 7
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(comboBox13.SelectedIndex.ToString());
            // Log index change 8
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timersecsession++;
            //MessageBox.Show(DateTime.Now.ToLongTimeString()+" ; " +DateTime.Now.ToLongDateString()+" ; "+timersecsession.ToString());
            //Log time for learning session
        }
    }
}
    


