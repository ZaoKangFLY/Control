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
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;

namespace gangway_controller
{
    /// <summary>
    /// BMQ.xaml 的交互逻辑
    /// </summary>

    public partial class BMQ : Window
    {
        int bmq_tmp = 0;
        SerialPort serialport2 = new SerialPort();
        public byte[] MCU = new byte[32];//8位无符号整数
        public byte[] SEND = new byte[32];//8位无符号整数
        private bool isInitialized = false; // 防止初始化就刷新
       

        public BMQ()
        {
            InitializeComponent();
            SerialInit();
            MCU[0] = 0xAA;
            MCU[2] = 0xFA;
            MCU[3] = 0xAF;
            check_HEX.IsChecked = true;
            // 初始化完成，设置标志为 true
            isInitialized = true;
        }
        //校验和
        public static byte CalculateSumChecksum(byte[] Cmd, int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex >= Cmd.Length || startIndex > endIndex)
            {
                throw new ArgumentOutOfRangeException("起始索引或结束索引超出数组范围或不合法。");
            }

            int sum = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                sum += Cmd[i];
            }

            // 仅取低8位
            return (byte)(sum & 0xFF);
        }
        //窗口关闭时关闭串口
        private void Window_Closed(object sender, EventArgs e)
        {
            if (serialport2.IsOpen)
            {
                serialport2.Close();
            }
        } 
        #region 串口
        private void SerialInit()
        {
            ComboBox_Baud.Items.Add("9600");//19200、38400、57600
            ComboBox_Baud.Items.Add("19200");
            ComboBox_Baud.Items.Add("38400");
            ComboBox_Baud.Items.Add("57600");
            ComboBox_Baud.Items.Add("115200");
            ComboBox_Baud.Items.Add("500000");
            ComboBox_Baud.Text = "115200";
            string[] name = SerialPort.GetPortNames();
            ComboBox_COM.Items.Clear();
            for (int i = 0; i < name.Length; i++)
            {
                ComboBox_COM.Items.Add(name[i]);
                ComboBox_COM.SelectedIndex = 0;
            }//选择串口号
            ComboBox_Baud_Copy.Items.Add("8bit");
            ComboBox_Baud_Copy1.Items.Add("None");
            ComboBox_Baud_Copy1.Items.Add("Even");
            ComboBox_Baud_Copy1.Items.Add("Odd");
            ComboBox_Baud_Copy2.Items.Add("1bit");
            ComboBox_Baud_Copy.Text = "8bit";
            ComboBox_Baud_Copy1.Text = "None";
            ComboBox_Baud_Copy2.Text = "1bit";
            serialport2.DataReceived += SerialPort_DataReceived;

        }//串口初始化  

        private void Shuaxin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialport2.IsOpen)
                {
                    serialport2.Close();
                    status.Fill = System.Windows.Media.Brushes.Red;
                    shuaxin.Content = "打开串口";

                    string[] name = SerialPort.GetPortNames();
                    ComboBox_COM.Items.Clear();
                    for (int i = 0; i < name.Length; i++)
                    {
                        ComboBox_COM.Items.Add(name[i]);
                        ComboBox_COM.SelectedIndex = 0;
                    }
                }
                else
                {
                    serialport2.PortName = ComboBox_COM.Text;
                    serialport2.BaudRate = int.Parse(ComboBox_Baud.Text.Trim());
                    if (ComboBox_Baud_Copy1.Text == "Even")
                    {
                        serialport2.Parity = Parity.Even;
                    }
                    else if (ComboBox_Baud_Copy1.Text == "Odd")
                    {
                        serialport2.Parity = Parity.Odd;
                    }
                    else
                    {
                        serialport2.Parity = Parity.None;
                    }
                    serialport2.Open();
                    status.Fill = System.Windows.Media.Brushes.Green;
                    shuaxin.Content = "关闭串口";
                }

            }
            catch (Exception e7)
            {
                MessageBox.Show(e7.Message);
            }
        }
        #endregion
        #region 通用
        private bool IsHexString(string input)
        {
            // 检查字符串是否仅包含十六进制字符，并且长度是偶数
            Regex hexRegex = new Regex("^[0-9A-Fa-f]+$");
            return hexRegex.IsMatch(input) && (input.Length % 2 == 0);
        }
        private /*async*/ void Send_Click_all(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (check_HEX.IsChecked == false)
            {
                string inputASCII = send_content.Text;
                if (string.IsNullOrEmpty(inputASCII))
                {
                    MessageBox.Show("请输入有效的ASCII字符串");
                    return;
                }
                try
                {
                    Send9.IsEnabled = false; // 禁用按钮

                    // 将 ASCII 字符串转换为 UTF-8 字节数组
                    byte[] utf8Bytes = Encoding.UTF8.GetBytes(inputASCII);

                    // 发送 UTF-8 字节数组
                    /* await Task.Run(() =>*/
                    serialport2.Write(utf8Bytes, 0, utf8Bytes.Length)/*)*/;

                    // MessageBox.Show("发送成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    Send9.IsEnabled = true; // 重新启用按钮
                }
            }
            else 
            {
                 string inputHEX = send_content.Text.Replace(" ", "");
                if (!IsHexString(inputHEX))
                {
                    MessageBox.Show("请输入有效的HEX字符串");
                    return;
                }


                try
                {
                    Send9.IsEnabled = false; // 禁用按钮

                    int byteCount = inputHEX.Length / 2;
                    for (int i = 0; i < byteCount; i++)
                    {
                        SEND[i] = Convert.ToByte(inputHEX.Substring(i * 2, 2), 16);
                    }

                    /* await Task.Run(() =>*/
                    serialport2.Write(SEND, 0, byteCount)/*)*/;

                    // MessageBox.Show("发送成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
                finally
                {
                    Send9.IsEnabled = true; // 重新启用按钮
                }
            }

           
        
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // 从串口读取现有的数据
                byte[] buffer = new byte[serialport2.BytesToRead]; // 创建一个数组，其大小等于当前可读取的字节数  
                int numBytesRead = serialport2.Read(buffer, 0, buffer.Length); // 读取数据到数组中  

                //转换为十六进制字符串  
                string hexData = BitConverter.ToString(buffer).Replace("-", " ");
                // 创建一个字符数组来存储转换后的ASCII字符  
               /* char[] asciiChars = new char[numBytesRead];*/

                this.Dispatcher.Invoke(() =>                // 在UI线程上执行以下代码，以便更新UI元素
                {
                    // 如果选中了HEX显示选项
                    if (check_HEX.IsChecked == true)
                    {
                        // 将接收到的数据转换为十六进制字符串
                        // string hexData = BitConverter.ToString(Encoding.ASCII.GetBytes(receivedData)).Replace("-", " ");
                        // 将转换后的十六进制字符串追加到接收文本框中
                        ReceiveTextBox.AppendText(hexData + "\n");
                    }
                    else if (check_ASCII.IsChecked == true)
                    {
                      /*  for (int i = 0; i < numBytesRead; i++)
                        {
                            asciiChars[i] = (char)buffer[i];
                        }*/
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, numBytesRead);
                        ReceiveTextBox.AppendText(receivedData + "\n");
                      /*  string asciiString = new string(asciiChars);
                        // 将接收到的原始数据追加到接收文本框中
                        ReceiveTextBox.AppendText(asciiString + "\n");*/

                    }
                    // 确保文本框自动滚动到最后一行
                    ReceiveTextBox.ScrollToEnd();
                });
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"接收数据时发生错误: {ex.Message}");
                });
            }
        }

        private void clc_Click_send(object sender, RoutedEventArgs e)
        {
            send_content.Clear();
        }

        private void check_HEX_Checked(object sender, RoutedEventArgs e)
        {
            if (check_HEX.IsChecked == true)
            {
                check_ASCII.IsChecked = false;
            }
        }

        private void check_ASCII_Checked(object sender, RoutedEventArgs e)
        {
            if (check_ASCII.IsChecked == true)
            {
                check_HEX.IsChecked = false;
            }
        }
        private void save_Click_content(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "ReceivedData",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;
                try
                {
                    File.WriteAllText(filename, ReceiveTextBox.Text);
                    MessageBox.Show("数据已保存");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存数据时发生错误: {ex.Message}");
                }
            }
        }

        private void clc_Click_content(object sender, RoutedEventArgs e)
        {
            ReceiveTextBox.Clear();
        }

     

       





        #endregion
        #region 主推

        private int _currentValue0 = 0; // 假设当前值为整数 
        private void Send_Click_zhutui(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                MessageBox.Show("FPGA板暂不支持输入发送模式，请用挡位调节！");
                /*  string inputText = mainmotor.Text;
                  if (inputText != "")
                  {
                      byte inputNum = Convert.ToByte(inputText); ;
                      if (inputNum > 0 && inputNum < 100)
                      {
                          MCU[1] = 0x01;
                          MCU[4] = 0x02;
                          MCU[8] = 0x24;
                          MCU[5] = inputNum;
                          if (MCU[6] == 0) { MCU[6] = 0x5A; }
                          if (MCU[7] == 0) { MCU[7] = 0x5A; }
                          try
                          {
                              if (check_ARM.IsChecked == false)
                              {
                                  serialport2.Write(MCU, 0, 9);
                              }
                              else
                              {
                                  serialport2.Write(MCU, 2, 7);
                              }
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                          }
                      }
                      else
                      {
                          MessageBox.Show("请输入有效的数字");
                      }
            }
            else
                {
                    MessageBox.Show("请输入有效的数字");
                }
            */
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void stop_Click_zhutui(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                MCU[1] = 0x01;
                MCU[4] = 0x02;
                MCU[8] = 0x24;
                MCU[5] = 0x0A;
                if (MCU[6] == 0) { MCU[6] = 0x5A; }
                if (MCU[7] == 0) { MCU[7] = 0x5A; }
                try
                {
                    if (check_ARM.IsChecked == false)
                    {
                        serialport2.Write(MCU, 0, 9);
                    }
                    else
                    {
                        serialport2.Write(MCU, 2, 7);
                    }
                    NumericTextBox0.Text = "0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        private async void DecrementButton_Click0(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                if (_currentValue0 > 0) // 防止减到负数（如果需要）  
                {
                    MCU[1] = 0x01;
                    MCU[4] = 0x02;
                    MCU[8] = 0x24;
                    if (MCU[6] == 0) { MCU[6] = 0x5A; }
                    if (MCU[7] == 0) { MCU[7] = 0x5A; }
                    _currentValue0--;
                    NumericTextBox0.Text = _currentValue0.ToString();
                    switch (_currentValue0)
                    {
                        case 0:
                            MCU[5] = 0x0A;
                            break;
                        case 1:
                            MCU[5] = 0x10;
                            break;
                        case 2:
                            MCU[5] = 0x20;
                            break;

                        default:
                            MCU[5] = 0x0A;
                            break;
                    }
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            await Task.Run(() => serialport2.Write(MCU, 0, 9));
                        }
                        else
                        {
                            await Task.Run(() => serialport2.Write(MCU, 2, 7));

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("已经最小值");
                }

            }
            else
            {
                MessageBox.Show("请打开串口");
            }

        }

        private async void IncrementButton_Click0(object sender, RoutedEventArgs e)
        {

            if (serialport2.IsOpen)
            {
                if (_currentValue0 < 3) // 防止减到负数（如果需要）  
                {
                    MCU[1] = 0x01;
                    MCU[4] = 0x02;
                    MCU[8] = 0x24;
                    if (MCU[6] == 0) { MCU[6] = 0x5A; }
                    if (MCU[7] == 0) { MCU[7] = 0x5A; }
                    _currentValue0++; // 假设没有上限（你可以根据需求设置）
                    NumericTextBox0.Text = _currentValue0.ToString();
                    switch (_currentValue0)
                    {
                        case 1:
                            MCU[5] = 0x10;
                            break;
                        case 2:
                            MCU[5] = 0x20;
                            break;
                        case 3:
                            MCU[5] = 0x30;
                            break;
                        default:
                            MCU[5] = 0x0A;
                            break;
                    }
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            await Task.Run(() => serialport2.Write(MCU, 0, 9));
                        }
                        else
                        {
                            await Task.Run(() => serialport2.Write(MCU, 2, 7));
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("已经最大值");
                }

            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

     


        #endregion
        #region 舵
    

   

        private void shuipingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isInitialized) return; // 如果未初始化完毕，不执行事件处理
            if (serialport2.IsOpen )
            {
                if (shuiping_slider.Value >= 60 && shuiping_slider.Value <= 120)
                {
                    horizontal.Text = ((int)shuiping_slider.Value).ToString();
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        private async void Thumb_DragCompleted_shuiping(object sender, DragCompletedEventArgs e)
        {
            if (shuiping_slider.Value >= 60 && shuiping_slider.Value <= 120)
            {
                // horizontal.Text = ((int)shuiping_slider.Value).ToString();
                if (serialport2.IsOpen)
                {
                    MCU[1] = 0x01;
                    MCU[4] = 0x02;
                    MCU[8] = 0x24;
                    if (MCU[5] == 0x00) { MCU[5] = 0x0A; }
                    if (MCU[7] == 0x00) { MCU[7] = 0x5A; }
                    MCU[6] = (byte)shuiping_slider.Value;
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            await Task.Run(() => serialport2.Write(MCU, 0, 9));
                        }
                        else
                        {
                            await Task.Run(() => serialport2.Write(MCU, 2, 7));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("请打开串口");
                }
            }
        }
        private /*async*/ void Send_Click_shuipingDuo(object sender, RoutedEventArgs e)
        {

            if (horizontal.Text != "")
            {
                if (int.TryParse(horizontal.Text, out int value))
                {
                    if (value >= 60 && value <= 120)
                    {
                        //shuiping_slider.ValueChanged -= shuipingSlider_ValueChanged;
                        shuiping_slider.Value = value;
                        // shuiping_slider.ValueChanged += shuipingSlider_ValueChanged;
                        //shuiping_slider.InvalidateVisual();
                        if (serialport2.IsOpen)
                        {


                            MCU[1] = 0x01;
                            MCU[4] = 0x02;
                            MCU[8] = 0x24;
                            if (MCU[5] == 0x00) { MCU[5] = 0x0A; }
                            if (MCU[7] == 0x00) { MCU[7] = 0x5A; }
                            MCU[6] = (byte)value;

                            try
                            {
                                Send3.IsEnabled = false;
                                if (check_ARM.IsChecked == false)
                                {
                                    /*await Task.Run(() => */serialport2.Write(MCU, 0, 9)/*)*/;

                                }
                                else
                                {
                                    /*await Task.Run(() => */serialport2.Write(MCU, 2, 7)/*)*/;

                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                            }
                            finally
                            {
                                Send3.IsEnabled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("请打开串口");
                        }

                    }
                    else
                    {
                        MessageBox.Show("请输入60到120之间的数值");
                    }
                }
                else
                {
                    MessageBox.Show("请输入有效的数字");
                }

            }

            else
            {
                MessageBox.Show("请输入有效的数字");
            }

        }

        private void chuizhiSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isInitialized) return; // 如果未初始化完毕，不执行事件处理
            if (serialport2.IsOpen)
            {
                if (shuiping_slider.Value >= 60 && shuiping_slider.Value <= 120)
                {
                    vertical.Text = ((int)chuizhi_slider.Value).ToString();
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        private async void Thumb_DragCompleted_chuizhi(object sender, DragCompletedEventArgs e)
        {
            if (shuiping_slider.Value >= 60 && shuiping_slider.Value <= 120)
            {
                if (serialport2.IsOpen)
                {
                    MCU[1] = 0x01;
                    MCU[4] = 0x02;
                    MCU[8] = 0x24;
                    if (MCU[5] == 0x00) { MCU[5] = 0x0A; }
                    if (MCU[6] == 0x00) { MCU[6] = 0x5A; }
                    MCU[7] = (byte)chuizhi_slider.Value;
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            await Task.Run(() => serialport2.Write(MCU, 0, 9));
                        }
                        else
                        {
                            await Task.Run(() => serialport2.Write(MCU, 2, 7));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("请打开串口");
                }
            }
        }
        private async void Send_Click_chuizhiDuo(object sender, RoutedEventArgs e)
        {

            if (vertical.Text != "")
            {
                if (int.TryParse(vertical.Text, out int value))
                {
                    if (value >= 60 && value <= 120)
                    {
                        //shuiping_slider.ValueChanged -= shuipingSlider_ValueChanged;
                        chuizhi_slider.Value = value;
                        // shuiping_slider.ValueChanged += shuipingSlider_ValueChanged;
                        //shuiping_slider.InvalidateVisual();
                        if (serialport2.IsOpen)
                        {


                            MCU[1] = 0x01;
                            MCU[4] = 0x02;
                            MCU[8] = 0x24;
                            if (MCU[5] == 0x00) { MCU[5] = 0x0A; }
                            if (MCU[6] == 0x00) { MCU[6] = 0x5A; }
                            MCU[7] = (byte)value;

                            try
                            {
                                if (check_ARM.IsChecked == false)
                                {
                                    await Task.Run(() => serialport2.Write(MCU, 0, 9));

                                }
                                else
                                {
                                    await Task.Run(() => serialport2.Write(MCU, 2, 7));

                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                            }
                        }
                        else
                        {
                            MessageBox.Show("请打开串口");
                        }

                    }
                    else
                    {
                        MessageBox.Show("请输入60到120之间的数值");
                    }
                }
                else
                {
                    MessageBox.Show("请输入有效的数字");
                }

            }

            else
            {
                MessageBox.Show("请输入有效的数字");
            }

        }


        private async void stop_Click_shuipingDuo(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                MCU[1] = 0x01;
                MCU[4] = 0x02;
                MCU[8] = 0x24;
                if (MCU[5] == 0x00) { MCU[5] = 0x0A; }
                MCU[6] = 0x5A;
                if (MCU[7] == 0x00) { MCU[7] = 0x5A; }

                try
                {
                    if (check_ARM.IsChecked == false)
                    {
                        await Task.Run(() => serialport2.Write(MCU, 0, 9));

                    }
                    else
                    {
                        await Task.Run(() => serialport2.Write(MCU, 2, 7));

                    }
                    shuiping_slider.Value = 90;
                    horizontal.Text = "0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private async  void stop_Click_chuizhiDuo(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                MCU[1] = 0x01;
                MCU[4] = 0x02;
                MCU[8] = 0x24;
                if (MCU[5] == 0x00) { MCU[5] = 0x0A; }
                if (MCU[6] == 0x00) { MCU[6] = 0x5A; }
                MCU[7] = 0x5A;
                try
                {
                    if (check_ARM.IsChecked == false)
                    {
                        await Task.Run(() => serialport2.Write(MCU, 0, 9));

                    }
                    else
                    {
                        await Task.Run(() => serialport2.Write(MCU, 2, 7));

                    }
                    chuizhi_slider.Value = 90;
                    vertical.Text = "0";
                 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        #endregion
        #region 侧推
        private void Send_Click_cetui(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未开发");
        }
        private void stop_Click_cetui(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未开发");
        }


        #endregion
        #region ARM
        public void ARMinit()
        {
            MCU[1] = 0x02;
            MCU[4] = 0x04;
            MCU[5] = 0x18;
            MCU[6] = 0x00;
            MCU[25] = 0xFE;
        }
     
        
        #region 功能
        private void ReamkeArm(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("串口未打卡!");
                return;
            }
            ARMinit();
            MCU[23] = 0xFF;
            int ans = 0xFA + 0xAF + 0x04 + 0x18;
            MCU[24] = (byte)ans;
            for (int i = 7; i < 24; i++)
            {
                MCU[i] = 0x00;
            }
            shouder.Text = "";
            dabi.Text = "";
            xiaobi.Text = "";
            wanbu.Text = "";
            try
            {
                RemakeAllArm.IsEnabled = false;
                if (check_ARM.IsChecked == false)
                {
                    /*await Task.Run(() =>*/
                    serialport2.Write(MCU, 0, 32)/*)*/;
                }
                else
                {
                    /*await Task.Run(() => */
                    serialport2.Write(MCU, 2, 24)/*)*/;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发送数据时发生错误: {ex.Message}");
            }
            finally
            {
                RemakeAllArm.IsEnabled = true;
            }
            MessageBox.Show("所有输入已经复位，请重新输入！");

        }
        private /*async*/ void StopArm(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("串口未打卡!");
                return;
            }
            ARMinit();
            MCU[23] = 0xFF;
            int ans = 0xFA + 0xAF + 0x04 + 0x18 + 0xFF;
            MCU[24] = (byte)ans;
            for (int i = 7; i < 23; i++)
            {
                MCU[i] = 0x00;
            }
            try
            {
                StopAllArm.IsEnabled = false;
                if (check_ARM.IsChecked == false)
                {
                    /*await Task.Run(() =>*/
                    serialport2.Write(MCU, 0, 32)/*)*/;
                }
                else
                {
                    /*await Task.Run(() => */
                    serialport2.Write(MCU, 2, 24)/*)*/;
                }
                // MessageBox.Show("已经停机，全部重新初始化");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发送数据时发生错误: {ex.Message}");
            }
            finally
            {
                StopAllArm.IsEnabled = true;
            }

            
           
        }
        private void SendArm(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            else if (shouder.Text == "" && dabi.Text == "" && xiaobi.Text == "" && wanbu.Text == "")
            {
                MessageBox.Show("请至少输入一个");
                return;
            }
            if (shouder.Text == "") { shouder.Text = "0"; }
            if (dabi.Text =="") { dabi.Text = "0"; }
            if (xiaobi.Text == "")  { xiaobi.Text = "0"; }
            if (wanbu.Text == "")  { wanbu.Text = "0"; }                 
            // 获取输入文本
            string shouderInput = shouder.Text;
            string dabiInput = dabi.Text;
            string xiaobiInput = xiaobi.Text;
            string wanbuInput = wanbu.Text;

            ARMinit();

            // 检查并转换所有浮点数输入
            bool isFloatConversionSuccessful = true;

            // 转换和检查函数
            void ConvertAndCheckInput(string inputText, int startIndex)
            {
                if (float.TryParse(inputText, out float floatValue))
                {
                    if (floatValue <= 180 && floatValue >= -180)
                    {
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);
                        Array.Copy(floatBytes, 0, MCU, startIndex, floatBytes.Length);
                    }
                    else
                    {
                        MessageBox.Show("超出范围！");
                        isFloatConversionSuccessful = false;
                        return;
                    }
                }
                else
                {
                    isFloatConversionSuccessful = false;
                    MessageBox.Show("请输入有效的浮点数。");
                    return;
                }
            }

            // 转换四个输入文本
            ConvertAndCheckInput(shouderInput, 6);
            if (!isFloatConversionSuccessful) return;

            ConvertAndCheckInput(dabiInput, 10);
            if (!isFloatConversionSuccessful) return;

            ConvertAndCheckInput(xiaobiInput, 14);
            if (!isFloatConversionSuccessful) return;

            ConvertAndCheckInput(wanbuInput, 18);
            if (!isFloatConversionSuccessful) return;

            // 计算校验和
            MCU[24] = CalculateSumChecksum(MCU, 2, 23);

            // 如果所有转换都成功，尝试通过串口发送数据
            if (isFloatConversionSuccessful)
            {
                try
                {
                    if (check_ARM.IsChecked == false)
                    {
                        serialport2.Write(MCU, 0, 32);
                    }
                    else
                    {
                        serialport2.Write(MCU, 2, 24);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
            }
           
        }
        #endregion
        #region 输入发送
        private void Send_Click_jian(object sender, RoutedEventArgs e)
        {
            //板子判断
            if (serialport2.IsOpen)
            {
                string inputText = shouder.Text;
                ARMinit();
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        if(floatValue <= 180 && floatValue >= -180)
                       { // 将浮点数转换为字节数组（IEEE 754格式）
                            byte[] floatBytes = BitConverter.GetBytes(floatValue);

                            // 将字节数组的内容复制到MCU数组，从索引6开始
                            Array.Copy(floatBytes, 0, MCU, 7, floatBytes.Length);
                            MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                            try
                            {
                                if (check_ARM.IsChecked == false)
                                {
                                    serialport2.Write(MCU, 0, 32);
                                }
                                else
                                {
                                    serialport2.Write(MCU, 2, 24);
                                }
                               
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                               
                            }
                        }
                        else
                        {
                            MessageBox.Show("超出范围！");
                            return;

                        }
                    }
                    else
                    {
                        
                        MessageBox.Show("请输入。");
                    }


                }
                catch (Exception ex)
                {
  
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
              
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void Send_Click_big(object sender, RoutedEventArgs e)
        {
            //板子判断
            if (serialport2.IsOpen)
            {
                string inputText = dabi.Text;
                ARMinit();
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        if (floatValue <= 180 && floatValue >= -180)
                        {
                            // 将浮点数转换为字节数组（IEEE 754格式）
                            byte[] floatBytes = BitConverter.GetBytes(floatValue);

                            // 将字节数组的内容复制到MCU数组，从索引6开始
                            Array.Copy(floatBytes, 0, MCU, 11, floatBytes.Length);
                            MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                            try
                            {
                                if (check_ARM.IsChecked == false)
                                {
                                    serialport2.Write(MCU, 0, 32);
                                }
                                else
                                {
                                    serialport2.Write(MCU, 2, 24);
                                }



                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"发送数据时发生错误: {ex.Message}");

                            }
                        }
                        else
                        {
                            MessageBox.Show("超出范围！");
                            return;
                        }
                    }
                    else
                    {
                       
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                   
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
          
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void Send_Click_samll(object sender, RoutedEventArgs e)
        {
            //板子判断
            if (serialport2.IsOpen)
            {
                string inputText = xiaobi.Text;
                ARMinit();
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        if (floatValue <= 180 && floatValue >= -180)
                        {
                            // 将浮点数转换为字节数组（IEEE 754格式）
                            byte[] floatBytes = BitConverter.GetBytes(floatValue);

                            // 将字节数组的内容复制到MCU数组，从索引6开始
                            Array.Copy(floatBytes, 0, MCU, 15, floatBytes.Length);
                            MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                            try
                            {
                                if (check_ARM.IsChecked == false)
                                {
                                    serialport2.Write(MCU, 0, 32);
                                }
                                else
                                {
                                    serialport2.Write(MCU, 2, 24);
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"发送数据时发生错误: {ex.Message}");

                            }
                        }
                         else
                        {
                            MessageBox.Show("超出范围！");
                            return;
                        }
                     }
                    else
                    {
                      
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                  
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
              
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void Send_Click_wan(object sender, RoutedEventArgs e)
        {
            //板子判断
            if (serialport2.IsOpen)
            {
                string inputText = wanbu.Text;
                ARMinit();
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        if (floatValue <= 180 && floatValue >= -180)
                        {
                            // 将浮点数转换为字节数组（IEEE 754格式）
                            byte[] floatBytes = BitConverter.GetBytes(floatValue);

                            // 将字节数组的内容复制到MCU数组，从索引6开始
                            Array.Copy(floatBytes, 0, MCU, 19, floatBytes.Length);
                            MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                            try
                            {
                                if (check_ARM.IsChecked == false)
                                {
                                    serialport2.Write(MCU, 0, 32);
                                }
                                else
                                {
                                    serialport2.Write(MCU, 2, 24);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                            }
                        }
                        else
                        {
                            MessageBox.Show("超出范围！");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void Send_Click_zhua_close(object sender, RoutedEventArgs e)
        {
            //板子判断
            if (serialport2.IsOpen)
            {
                ARMinit();
                MCU[23] = 0x0A;
                MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                try
                {
                    serialport2.Write(MCU, 0, 32);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void Send_Click_zhua_open(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                ARMinit();
                MCU[23] = 0x0F;
                MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                try
                {
                    if (check_ARM.IsChecked == false)
                    {
                        serialport2.Write(MCU, 0, 32);
                    }
                    else
                    {
                        serialport2.Write(MCU, 2, 24);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        #endregion
        #region 阶梯增减
        private void IncrementButton_Click_jian(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIshouder.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            //防止shouder.Text为空
            if (shouder.Text == "") { shouder.Text = "0"; }
            // if (float.TryParse(shouder.Text, out float floatValue))
            // 尝试将DIshouder.Text和shouder.Text转换为数  
            if (float.TryParse(DIshouder.Text, out number1) && float.TryParse(shouder.Text, out number2))
            {
                // 计算两个数的和  
                float sum = +number1 + number2;
                if (sum >= -180 && sum <= 180)
                {   // 将和转换为字符串并更新到shouder.Text中（或者您可以选择更新到DIshouder.Text或其他文本框）  
                    shouder.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);
                    // 将字节数组的内容复制到MCU数组，从索引6开始
                    Array.Copy(floatBytes, 0, MCU, 7, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                // 如果无法转换，可以显示一个错误消息或执行其他错误处理  
                MessageBox.Show("转换出错。");
                return;
            }


        }

        private void DecrementButton_Click_jian(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIshouder.Text == "")
            { 
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            //防止shouder.Text为空
            if (shouder.Text == "") { shouder.Text = "0"; }
            // if (float.TryParse(shouder.Text, out float floatValue))
                // 尝试将DIshouder.Text和shouder.Text转换为数  
            if (float.TryParse(DIshouder.Text, out number1) && float.TryParse(shouder.Text, out number2))
            {
            // 计算两个数的和  
                float sum = -number1 + number2;
                if ( sum >= -180 && sum <= 180 )
                {   // 将和转换为字符串并更新到shouder.Text中（或者您可以选择更新到DIshouder.Text或其他文本框）  
                    shouder.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);
                    // 将字节数组的内容复制到MCU数组，从索引6开始
                    Array.Copy(floatBytes, 0, MCU, 7, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
             }
            else
            {
                // 如果无法转换，可以显示一个错误消息或执行其他错误处理  
                MessageBox.Show("转换出错。");
                return;
            }
      

        }

        private void IncrementButton_Click_big(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIdabi.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;  
            if (dabi.Text == "") { dabi.Text = "0"; }  
            if (float.TryParse(DIdabi.Text, out number1) && float.TryParse(dabi.Text, out number2))
            {
                // 计算两个数的和  
                float sum = number1 + number2;
                if (sum >= -180 && sum <= 180)
                {   
                    dabi.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);
                   
                    Array.Copy(floatBytes, 0, MCU, 11, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("转换出错。");
                return;
            }
        }

        private void DecrementButton_Click_big(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIdabi.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            if (dabi.Text == "") { dabi.Text = "0"; }
            if (float.TryParse(DIdabi.Text, out number1) && float.TryParse(dabi.Text, out number2))
            {
                // 计算两个数的和  
                float sum = - number1 + number2;
                if (sum >= -180 && sum <= 180)
                {
                    dabi.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);

                    Array.Copy(floatBytes, 0, MCU, 11, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("转换出错。");
                return;
            }
        }

        private void IncrementButton_Click_small(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIxiaobi.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            if (xiaobi.Text == "") { xiaobi.Text = "0"; }
            if (float.TryParse(DIxiaobi.Text, out number1) && float.TryParse(xiaobi.Text, out number2))
            {
                // 计算两个数的和  
                float sum = number1 + number2;
                if (sum >= -180 && sum <= 180)
                {
                    xiaobi.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);

                    Array.Copy(floatBytes, 0, MCU, 15, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("转换出错。");
                return;
            }
        }

        private void DecrementButton_Click_samll(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIxiaobi.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            if (xiaobi.Text == "") { xiaobi.Text = "0"; }
            if (float.TryParse(DIxiaobi.Text, out number1) && float.TryParse(xiaobi.Text, out number2))
            {
                // 计算两个数的和  
                float sum = - number1 + number2;
                if (sum >= -180 && sum <= 180)
                {
                    xiaobi.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);

                    Array.Copy(floatBytes, 0, MCU, 15, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("转换出错。");
                return;
            }
        }

        private void IncrementButton_Click_wan(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIwanbu.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            if (wanbu.Text == "") { wanbu.Text = "0"; }
            if (float.TryParse(DIwanbu.Text, out number1) && float.TryParse(wanbu.Text, out number2))
            {
                // 计算两个数的和  
                float sum = number1 + number2;
                if (sum >= -180 && sum <= 180)
                {
                    wanbu.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);

                    Array.Copy(floatBytes, 0, MCU, 19, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("转换出错。");
                return;
            }
        }

        private void DecrementButton_Click_wan(object sender, RoutedEventArgs e)
        {
            if (!serialport2.IsOpen)
            {
                MessageBox.Show("请打开串口");
                return;
            }
            if (DIwanbu.Text == "")
            {
                MessageBox.Show("请输入");
                return;
            }
            float number1;
            float number2;
            if (wanbu.Text == "") { wanbu.Text = "0"; }
            if (float.TryParse(DIwanbu.Text, out number1) && float.TryParse(wanbu.Text, out number2))
            {
                // 计算两个数的和  
                float sum = - number1 + number2;
                if (sum >= -180 && sum <= 180)
                {
                    wanbu.Text = sum.ToString();
                    ARMinit();
                    // 将浮点数转换为字节数组（IEEE 754格式）
                    byte[] floatBytes = BitConverter.GetBytes(sum);

                    Array.Copy(floatBytes, 0, MCU, 19, floatBytes.Length);
                    MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                    try
                    {
                        if (check_ARM.IsChecked == false)
                        {
                            serialport2.Write(MCU, 0, 32);
                        }
                        else
                        {
                            serialport2.Write(MCU, 2, 24);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("超出范围！");
                    return;
                }
            }
            else
            {
                MessageBox.Show("转换出错。");
                return;
            }
        }




        #endregion

        #endregion


    }
}
