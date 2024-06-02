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
using System.IO.Ports;//串口的名称空间
using System.IO;
using System.Windows.Controls.Primitives;

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
        private bool isFloatConversionSuccessful = false;
        private bool isInitialized = false; // 初始化标志
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

        public BMQ()
        {
            InitializeComponent();
            SerialInit();
            MCU[0] = 0xAA;
            MCU[2] = 0xFA;
            MCU[3] = 0xAF;
            // 初始化完成，设置标志为 true
            isInitialized = true;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (serialport2.IsOpen)
            {
                serialport2.Close();
            }
        }

        private void SerialInit()
        {
            ComboBox_Baud.Items.Add("9600");//19200、38400、57600
            ComboBox_Baud.Items.Add("19200");
            ComboBox_Baud.Items.Add("38400");
            ComboBox_Baud.Items.Add("57600");
            ComboBox_Baud.Items.Add("115200");
            ComboBox_Baud.Items.Add("500000");
            ComboBox_Baud.Text = "19200";
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
            ComboBox_Baud_Copy1.Text = "Even";
            ComboBox_Baud_Copy2.Text = "1bit";


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
        // AA 01 FA AF 02 20 5A 5A 24
        //AA 02 FA AF 04 18 00 00 00 00 00 00 00 B4 42 00 00 B4 42 00 00 00 00 00 B1 FE 00 00 00 00 08 17
        private void Send_Click_all(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未开发");
        }
        #region 主推
        private void mainmotor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }

        }
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



        private void send_content_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }


        #endregion
        #region 舵
        private void horizontal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void vertical_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }

        private void shuipingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isInitialized) return; // 如果未初始化完毕，不执行事件处理
            if (serialport2.IsOpen)
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
        private async void Send_Click_shuipingDuo(object sender, RoutedEventArgs e)
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

        private async void StopArm(object sender, RoutedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;
                MCU[23] = 0xFF;
                MCU[25] = 0xFE;
                int ans = 0xFA + 0xAF + 0x04 + 0x18 + 0xFF;
                MCU[24] = (byte)ans;
                for (int i = 7; i < 23; i++)
                {
                    MCU[i] = 0x00;
                }
                try
                {
                    
                    if (check_ARM.IsChecked == false)
                    {
                        await Task.Run(() => serialport2.Write(MCU, 0, 32));
                    }
                    else
                    {
                        await Task.Run(() => serialport2.Write(MCU, 2, 24));
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
        private void shouder_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        private void dabi_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        private void xiaobi_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }
        private void wanbu_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (serialport2.IsOpen)
            {
            }
            else
            {
                MessageBox.Show("请打开串口");
            }
        }


        private void Send_Click_jian(object sender, RoutedEventArgs e)
        {
            //板子判断
            if (serialport2.IsOpen)
            {
                string inputText = shouder.Text;
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;

                MCU[25] = 0xFE;
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        // 将浮点数转换为字节数组（IEEE 754格式）
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);

                        // 将字节数组的内容复制到MCU数组，从索引6开始
                        Array.Copy(floatBytes, 0, MCU, 7, floatBytes.Length);
                        MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                        isFloatConversionSuccessful = true;

                    }
                    else
                    {
                        isFloatConversionSuccessful = false;
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                    isFloatConversionSuccessful = false;
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
                if (isFloatConversionSuccessful == true)
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
                        isFloatConversionSuccessful = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        isFloatConversionSuccessful = false;
                    }
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
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;
                MCU[25] = 0xFE;
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        // 将浮点数转换为字节数组（IEEE 754格式）
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);

                        // 将字节数组的内容复制到MCU数组，从索引6开始
                        Array.Copy(floatBytes, 0, MCU, 11, floatBytes.Length);
                        MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                        isFloatConversionSuccessful = true;

                    }
                    else
                    {
                        isFloatConversionSuccessful = false;
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                    isFloatConversionSuccessful = false;
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
                if (isFloatConversionSuccessful == true)
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
                        MessageBox.Show("发送成功");

                        isFloatConversionSuccessful = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        isFloatConversionSuccessful = false;
                    }
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
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;
                MCU[25] = 0xFE;
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        // 将浮点数转换为字节数组（IEEE 754格式）
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);

                        // 将字节数组的内容复制到MCU数组，从索引6开始
                        Array.Copy(floatBytes, 0, MCU, 15, floatBytes.Length);
                        MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                        isFloatConversionSuccessful = true;

                    }
                    else
                    {
                        isFloatConversionSuccessful = false;
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                    isFloatConversionSuccessful = false;
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
                if (isFloatConversionSuccessful == true)
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
                        isFloatConversionSuccessful = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        isFloatConversionSuccessful = false;
                    }
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
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;
                MCU[25] = 0xFE;
                try
                {
                    // 尝试将字符串转换为浮点数
                    if (float.TryParse(inputText, out float floatValue))
                    {
                        // 将浮点数转换为字节数组（IEEE 754格式）
                        byte[] floatBytes = BitConverter.GetBytes(floatValue);

                        // 将字节数组的内容复制到MCU数组，从索引6开始
                        Array.Copy(floatBytes, 0, MCU, 19, floatBytes.Length);
                        MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                        isFloatConversionSuccessful = true;

                    }
                    else
                    {
                        isFloatConversionSuccessful = false;
                        MessageBox.Show("请输入有效的浮点数。");
                    }


                }
                catch (Exception ex)
                {
                    isFloatConversionSuccessful = false;
                    MessageBox.Show($"转换发生错误: {ex.Message}");
                }
                if (isFloatConversionSuccessful == true)
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
                        isFloatConversionSuccessful = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"发送数据时发生错误: {ex.Message}");
                        isFloatConversionSuccessful = false;
                    }
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
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;
                MCU[23] = 0x0A;
                MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                MCU[25] = 0xFE;

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
                MCU[1] = 0x02;
                MCU[4] = 0x04;
                MCU[5] = 0x18;
                MCU[6] = 0x00;
                MCU[23] = 0x0F;
                MCU[24] = CalculateSumChecksum(MCU, 2, 23);
                MCU[25] = 0xFE;
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

   


    }
}
