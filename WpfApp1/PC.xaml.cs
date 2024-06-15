using System;
using System.IO.Ports;
using System.Windows;

namespace gangway_controller
{
    public partial class NewWindow : Window
    {
        private SerialPort _serialPort;
        public byte[] PCDown = { 0xAA,0x0A, 0x02, 0x01, 0x03, 0x05, 0x07, 0x09, 0x0B, 0x0D, 0x0F, 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0, 0xF0, 0x56, 0x78, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public byte[] CanDown = { 0xAA, 0x0A, 0x02, 0x01, 0x03, 0x05, 0x07, 0x09, 0x0B, 0x0D, 0x0F, 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0, 0xF0, 0x13, 0x57, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public byte[] ReApp = { 0xAA, 0x0A, 0x02, 0x01, 0x03, 0x05, 0x07, 0x09, 0x0B, 0x0D, 0x0F, 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0, 0xF0, 0x12, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public byte[] RePC = { 0xAA, 0x0A, 0x02, 0x01, 0x03, 0x05, 0x07, 0x09, 0x0B, 0x0D, 0x0F, 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0, 0xF0, 0x9A, 0xBC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public byte[] FastDown = { 0xAA, 0x0A, 0x02, 0x01, 0x03, 0x05, 0x07, 0x09, 0x0B, 0x0D, 0x0F, 0x10, 0x30, 0x50, 0x70, 0x90, 0xB0, 0xD0, 0xF0, 0x24, 0x68, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public NewWindow(SerialPort serialPort)
        {
            InitializeComponent();
            _serialPort = serialPort;
        }

        private void PCDown_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                { 
                    PCdown.IsEnabled = false;
                    _serialPort.Write(PCDown, 0, PCDown.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    PCdown.IsEnabled = true;
                }

            }
            else
            {
                MessageBox.Show("串口未打开");
            }
        }

        private void FastDown_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    Fastdown.IsEnabled = false;
                    _serialPort.Write(FastDown, 0, FastDown.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    Fastdown.IsEnabled = true;
                }

            }
            else
            {
                MessageBox.Show("串口未打开");
            }   
        }

        private void restartPC_Click(object sender, RoutedEventArgs e)
        {
            if(_serialPort != null && _serialPort.IsOpen    )
            {
                try
                {
                    PCre.IsEnabled = false;
                    _serialPort.Write(RePC, 0, RePC.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    PCre.IsEnabled = true;
                }
            }
            else
            {
                MessageBox.Show("串口未打开");
            }
        }

        private void CancellDown_Click(object sender, RoutedEventArgs e)
        {
            if(_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    Candown.IsEnabled = false;
                    _serialPort.Write(CanDown, 0, CanDown.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    Candown.IsEnabled = true;
                }
            }
            else
            {
                MessageBox.Show("串口未打开");
            }   
        }

        private void restartAPP_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    Reapp.IsEnabled = false;
                    _serialPort.Write(ReApp, 0, ReApp.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    Reapp.IsEnabled = true;
                }
            }
            else
            {
                MessageBox.Show("串口未打开");
            }   
        }
    }

}
