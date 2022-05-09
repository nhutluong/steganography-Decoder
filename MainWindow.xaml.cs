using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace SecretMessageDecoder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }//end main

        //instance of filepath to use in events
        public string filePath = "";

        //instance of ppm decoder to use in events
        PPMDecoder PPM = new PPMDecoder();

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            //create instance of openfile dialog 
            OpenFileDialog ofdTemp = new OpenFileDialog();

            //filters only the desired file types 
            ofdTemp.Filter = "PPM Files|*.ppm";

            //displays dialog box and checks if opened 
            bool fileSelected = ofdTemp.ShowDialog() == true;

            //keep record of file path 
            filePath = ofdTemp.FileName;

            //if user selected a file then load the image and send to imagebox
            if (fileSelected)
            {//then
                PPM = new PPMDecoder(filePath);                

                ImageBox.Source = PPM.MakeBitmap();
            }//end if
        }//end event

        private void Button_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            //check for imagebox = null
            if (ImageBox.Source == null)
            {
                MessageBox.Show("Upload a PPM image to decrypt message");
            }
            else 
            {
                string decryptedMessage = PPMDecoder.extractText(PPM);
                //debugging, shows encoded message
                txtBox.Text = PPMDecoder.extractText(PPM);
            }//end if
            
        }//end event

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            //SETUP INSTRUCTIONS
            string content =
                "• Open the 'File' drop down menu and click 'Open'\n\n" +
                "• Search for the appropriate .PPM file (P3 or P6) to open\n\n" +
                "• Click the 'Decrypt'\n" +
                "(You may need to wait some depending on how large the file is)\n\n" +
                "• Once the operation is complete your message will appear in the designated 'Decoded Message box'";

            //GIVE WINDOW TITLE
            string header = "Instructions";

            //give params to messagebox
            MessageBox.Show(content, header);
        }//end event
    }//end class
}//end namespace
