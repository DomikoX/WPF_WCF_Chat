using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChatClient.Properties;

namespace ChatClient
{
    public static class RichTextBoxExtension
    {
        public static void AppendText(this RichTextBox box, string userName, string text, bool privateMessage = false)
        {
            Append(box, userName, text, privateMessage);
        }

        public static void AppendImage(this RichTextBox box, string userName, byte[] image, bool privateMessage = false)
        {
            ImageSource source = ConvertByteArrayToBitmapImage(image);
            Append(box, userName, source, privateMessage);
        }

        private static void Append(this RichTextBox box, string userName, object objectToAppend,
            bool privateMessage = false)
        {
            var fontColor = Settings.Default.FontColor;
            var backColor = Settings.Default.BackColor;
            var fontSize = Settings.Default.FontSize;

            var back = privateMessage
                ? (SolidColorBrush) new BrushConverter().ConvertFromString(backColor)
                : Brushes.Transparent;
            var front = (SolidColorBrush) new BrushConverter().ConvertFromString(fontColor);

            var paragraph = new Paragraph
            {
                LineHeight = 1,
                Background = back
            };
            var time = new Run($"{DateTime.Now:T} ")
            {
                Foreground = Brushes.Black,
                FontSize = fontSize,
                BaselineAlignment = BaselineAlignment.Top
            };
            var name = new Run($"{userName}: ")
            {
                Foreground = Brushes.Blue,
                FontSize = fontSize,
                FontWeight = FontWeights.Bold,
                BaselineAlignment = BaselineAlignment.Top
            };


            paragraph.Inlines.Add(time);
            paragraph.Inlines.Add(name);

            if (objectToAppend is string)
            {
                var message = new Run((string) objectToAppend)
                {
                    Foreground = front,
                    FontSize = fontSize,
                    BaselineAlignment = BaselineAlignment.Top
                };
                paragraph.Inlines.Add(message);
            }
            else if (objectToAppend is BitmapImage)
            {
                var img = new Image
                {
                    Source = (ImageSource) objectToAppend,
                    MaxHeight = 200
                };
                paragraph.Inlines.Add(img);
            }
            else
            {
                return;
            }

            box.Document.Blocks.Add(paragraph);
            box.ScrollToEnd();
        }


        private static BitmapImage ConvertByteArrayToBitmapImage(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }
    }
}