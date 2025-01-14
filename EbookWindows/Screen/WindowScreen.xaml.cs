﻿using EbookWindows.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace EbookWindows.Screen
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// }

    public class TodoItem
    {
        public string Title { get; set; }
        public int Completion { get; set; }
    }
    public partial class WindowScreen : Window
    {
        public WindowScreen()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight; 

            //lbTodoList.ItemsSource = items;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        private void DragStart(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState != WindowState.Normal)
                {   
                    double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                    double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                    double percentVertical = e.GetPosition(this).Y / ActualHeight;
                    double targetVertical = RestoreBounds.Height * percentVertical;

                    var lMousePosition = GetMousePosition();
                    if (Left<0)
                        Left = lMousePosition.X - targetHorizontal;
                    else
                        Left = lMousePosition.X - targetHorizontal;
                    if (Top < 0)
                        Top = lMousePosition.Y - targetVertical;
                    else
                        Top = lMousePosition.Y - targetVertical;
                    WindowState = WindowState.Normal;
                }
                this.DragMove();
            }
        }
        private void Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Minimize(object sender, RoutedEventArgs e)
        {

            this.WindowState = WindowState.Minimized;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DragStart1(object sender, QueryContinueDragEventArgs e)
        {

        }
        public async void OpenDetailScreen(ShelfTag x)
        {
            StartLoading();
            await Task.Run(()=>detailScreen.LoadData(x));
            ShelfGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Visible;
            EndLoading();
        }
        public async void OpenDetailScreen(string url)
        {
            StartLoading();
            await Task.Run(() => detailScreen.LoadData(url));
            ShelfGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Visible;
            EndLoading();
        }

        public async void OpenComicReadingScreen()
        {
            await Task.Run(()=>comicReadingScreen.LoadData());
            ShelfGrid.Visibility = Visibility.Collapsed;
            detailScreen.Visibility = Visibility.Collapsed;
            comicReadingScreen.Visibility = Visibility.Visible;
        }
        private void ReturnHome_Click(object sender, RoutedEventArgs e)
        {
            if(ShelfGrid.Visibility != Visibility.Visible)
            {
                ShelfGrid.Visibility = Visibility.Visible;
                if (detailScreen.Visibility == Visibility.Visible)
                {
                    detailScreen.Visibility = Visibility.Collapsed;
                    return;
                }
                else if (pdfReadingScreen.Visibility == Visibility.Visible)
                {
                    pdfReadingScreen.homeBtn_Click(sender,e);
                    return;
                }
                else if (epubReadingScreen.Visibility == Visibility.Visible)
                {
                    epubReadingScreen.Visibility = Visibility.Collapsed;
                    return;
                }
                else
                {
                    //do nothing
                }
            }
            else
            {
                //do nothing
            }


        }

        public void ReturnFromReadingScreen_Click(object sender, RoutedEventArgs e)
        {
            ShelfGrid.Visibility = Visibility.Visible;
            pdfReadingScreen.Visibility = Visibility.Collapsed;
        }


        private void filePathChanged(string filePath, string fileExtension)
        {
            if (fileExtension.Equals(".pdf"))
            {
                pdfReadingScreen.LoadData(filePath);
                ShelfGrid.Visibility = Visibility.Collapsed;
                pdfReadingScreen.Visibility = Visibility.Visible;


            }
            else if (fileExtension.Equals(".epub"))
            {
                epubReadingScreen.ReadFile(filePath);
                ShelfGrid.Visibility = Visibility.Collapsed;
                epubReadingScreen.Visibility = Visibility.Visible;
            }
            else
            {
                //notthing
            }
           
        }

        private void addMoreBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var popupEbookScreen = new PopupEbookScreen();
            popupEbookScreen.BrowserEvent += filePathChanged;
            popupEbookScreen.ShowDialog();
            


        }
        public void StartLoading()
        {
                LoadingGrid.Visibility = Visibility.Visible;
        }
        public void EndLoading()
        {
                LoadingGrid.Visibility = Visibility.Collapsed;
        }

       
    }
}
      