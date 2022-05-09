using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace SecretMessageDecoder
{
    internal class PPMDecoder
    {
        private string _readFirstLine;

        //list to hold value of the encoded list
        public List<string> encodedList = new List<string>();

        //list to hold ppm p3 file for saving
        public List<string> p3List = new List<string>();

        //list to hold ppm p6 file for saving
        public byte[] p6List;

        //BITMAP'S SIZE.
        private int _width;
        private int _height;

        //PIXEL ARRAY.
        private byte[] _pixelData;

        //NUMBER OF BYTES PER ROW.
        private int _stride;

        //properties
        public int Width
        {
            get { return _width; }
        }//end property
        public int Height
        {
            get { return _height; }
        }//end property

        //constructors
        public PPMDecoder()
        {
            //no paramaters
        }//end constructor

        public PPMDecoder(string filePath)
        {
            //FileType(filePath);
            FileType(filePath);
        }//end constructor

        //methods
        private void FileType(string filePath)
        {
            _readFirstLine = File.ReadLines(filePath).First();

            if (_readFirstLine.Contains("P3"))
            {
                LoadP3(filePath);
            }
            else if (_readFirstLine.Contains("P6"))
            {
                LoadP6(filePath);
            }
            else
            {
                throw new Exception("Incorrect image format");
            }//end if
        }//end method
        
        public void LoadP3(string filePath)
        {
            //reads the file
            StreamReader fileReader = new StreamReader(filePath);

            //read first 4 lines of ppm file and assign them
            string ppmType = fileReader.ReadLine();
            string ppmComment = fileReader.ReadLine();
            string ppmSize = fileReader.ReadLine();
            string ppmRGBMax = fileReader.ReadLine();

            //add first 4 lines to list
            p3List.Add(ppmType.ToString());
            p3List.Add(ppmComment.ToString());
            p3List.Add(ppmSize.ToString());
            p3List.Add(ppmRGBMax.ToString());

            //split values of 3rd line and store the values to appropriate variables
            string[] fileValues = ppmSize.Split();
            _width = Convert.ToInt32(fileValues[0]);
            _height = Convert.ToInt32(fileValues[1]);
            int counter = 0;

            //get pixel data
            _pixelData = new byte[_width * _height * 4];

            //calculate stride
            _stride = _width * 4;
                        
            if (ppmRGBMax.Contains("255"))
            {
                //retrieve remaining bytes
                while (!fileReader.EndOfStream)
                {
                    //new instance of color
                    Color imgColor = new Color();

                    //convert to int
                    int blue = Convert.ToInt32(fileReader.ReadLine());
                    int green = Convert.ToInt32(fileReader.ReadLine());
                    int red = Convert.ToInt32(fileReader.ReadLine());

                    //reads colors from file, convert to byte, adds to list
                    imgColor.R = Convert.ToByte(red);
                    p3List.Add(red.ToString());

                    imgColor.G = Convert.ToByte(green);
                    p3List.Add(green.ToString());

                    imgColor.B = Convert.ToByte(blue);
                    p3List.Add(blue.ToString());

                    //set color in position
                    _pixelData[counter++] = imgColor.R;
                    _pixelData[counter++] = imgColor.G;
                    _pixelData[counter++] = imgColor.B;
                    _pixelData[counter++] = 255;

                }//end while
            }//end if   

            //close stream reader
            fileReader.Close();
        }//end method

        public void LoadP6(string filePath)
        {
            //byte array to hold data
            byte[] fileBytes = File.ReadAllBytes(filePath);

            //add bytes to list
            p6List = fileBytes;
            encodedList.Add(p6List.ToString());

            //declare instance of streamreader, read file
            StreamReader fileReader = new StreamReader(filePath);
            string counter = string.Empty;

            //line 1, read ppm type
            string ppmType = fileReader.ReadLine();
            if (!ppmType.Contains("P6"))
            {
                throw new Exception("Exception: could not read. Not ppm P6 format");
            }//end if

            counter += ppmType + "\n";

            //line 2, read line and skip
            string ppmComment = fileReader.ReadLine();
            counter += ppmComment + "\n";


            //line 3, get image width and height
            string ppmSize = fileReader.ReadLine();
            string[] ppmDimensionValues = ppmSize.Split(" ");

            _width = Convert.ToInt32(ppmDimensionValues[0]);
            _height = Convert.ToInt32(ppmDimensionValues[1]);

            counter += ppmSize + "\n";

            //line 4, read max rgb
            string ppmRGBMax = fileReader.ReadLine();
            counter += ppmRGBMax + "\n";

            //close streamreader
            fileReader.Close();

            //get pixel data
            _pixelData = new byte[_width * _height * 4];

            //calculate stride
            _stride = _width * 4;

            //line 5
            int count = 0;
            //new instance of binaryreader, read file
            BinaryReader binaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open));

            //loop to index position
            while (count < counter.Length)
            {
                char read = binaryReader.ReadChar();
                count++;
            }//end while

            //reset count
            count = 0;

            //determine if end of file reached, current position compared to pixel lenght of image
            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            {
                //new instance of color
                Color imgColor = new Color();

                //reads colors from file, convert to byte
                imgColor.B = binaryReader.ReadByte();
                
                imgColor.G = binaryReader.ReadByte();
                
                imgColor.R = binaryReader.ReadByte();
                
                //set color in position
                _pixelData[count++] = imgColor.R;
                _pixelData[count++] = imgColor.G;
                _pixelData[count++] = imgColor.B;
                _pixelData[count++] = 255;
            }//end while

            //close binaryreader
            binaryReader.Close();
        }//end method

        public string decryption()
        {

            var characterValue = 0; // Have a variable to store the ASCII value of the character

            string encryptedText = string.Empty; // Have a variable to store the encrypted text

            var ascii = new List<int>(); // Have a collection to store the collection of ASCII

           

            for (int row = 0; row < _width; row++) // Indicates row number
            {
                for (int column = 0; column < _height; column++) // Indicate column number
                {

                    var color = GetPixelColor(row, column); // Get the pixel from each and every row and column

                    ascii.Add(255 - color.A); // Get the ascii value from A value since 255 is default value

                }//end for
            }//end for

            for (int i = 0; i < ascii.Count; i++)
            {

                characterValue = 0;

                characterValue += ascii[i] * 1000; // Get the first digit of the ASCII value of the encrypted character

                i++;

                characterValue += ascii[i] * 100; // Get the second digit of the ASCII value of the encrypted character

                i++;

                characterValue += ascii[i] * 10;  // Get the first third digit of the ASCII value of the encrypted character

                i++;

                characterValue += ascii[i]; // Get the first fourth digit of the ASCII value of the encrypted character

                if (characterValue != 0)
                {


                    encryptedText += char.ConvertFromUtf32(characterValue); // Convert the ASCII characterValue into character
                }//end if
            }//end for

            return encryptedText; // Showing the encrypted message in message box

        }//end method

        public static string extractText(PPMDecoder ppm)
        {
            //color index to hold current pixel processed
            int colorUnitIndex = 0;
            int charValue = 0;

            //holds the text that will be extracted from the image
            string extractedText = String.Empty;

            //pass through the height
            for (int i = 0; i < ppm.Height; i++)
            {
                //pass through the width
                for (int j = 0; j < ppm.Width; j++)
                {
                    Color pixel = ppm.GetPixelColor(j, i);

                    // for each pixel, pass through its elements (RGB)
                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    //get the LSB from the pixel element 
                                    //then add one bit to the right of the current character             
                                    //replace the added bit (which value is by default 0) with
                                    //the LSB of the pixel element, simply by addition
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }//end switch

                        //increment color index
                        colorUnitIndex++;

                        //if 8 bits has been added, then add the current character to the extracted text
                        if (colorUnitIndex % 8 == 0)
                        {
                            //reverse? since each time the process happens on the right (so RGB is not BGR)
                            charValue = reverseBits(charValue);

                            //can only be 0 if it is the stop character (the 8 zeros)
                            if (charValue == 0)
                            {
                                return extractedText;
                            }//end if

                            //convert the character value from int to char
                            char text = (char)charValue;

                            //add the current character to the result text
                            extractedText += text.ToString();
                        }//end if
                    }//end for
                }//end for
            }//end for

            return extractedText;
        }//end method

        public static int reverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }//end for

            return result;
        }//end method

        public Color GetPixelColor(int x, int y)
        {
            //GET PIXEL DATA
            byte[] pixelComponentData = GetPixelData(x, y);

            //CREAT COLOR INSTANCE
            Color returnColor = new Color();

            //POPULATE COLOR INSTANCE DATA THEN RETURN
            returnColor.R = pixelComponentData[0];
            returnColor.G = pixelComponentData[1];
            returnColor.B = pixelComponentData[2];
            returnColor.A = pixelComponentData[3];

            return returnColor;
        }//end method

        public byte[] GetPixelData(int x, int y)
        {
            //STARTING PIXEL INDEX
            int index = y * _stride + x * 4;

            //GET PIXEL COMPONENT VALUES 
            byte blu = _pixelData[index++];// ++ to march forward to get next component 
            byte grn = _pixelData[index++];
            byte red = _pixelData[index++];
            byte alp = _pixelData[index];

            //RETURN DATA
            return new byte[] { red, grn, blu, alp };
        }//end method

        public WriteableBitmap MakeBitmap()
        {
            // Create the WriteableBitmap.
            int dpi = 96;

            WriteableBitmap wbitmap = new WriteableBitmap(_width, _height, dpi, dpi, PixelFormats.Bgra32, null);

            // Load the pixel data.
            Int32Rect rect = new Int32Rect(0, 0, _width, _height);
            wbitmap.WritePixels(rect, _pixelData, _stride, 0);

            // Return the bitmap.
            return wbitmap;
        }//end method

    }//end class
}//end namespace
