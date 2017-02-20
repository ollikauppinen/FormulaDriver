using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Gaming.Input;
using Windows.UI.Popups;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AllcodeBuggyXbox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        float x = 0, y = 0;
        double right, left;
        string buttonPressed;
        char portNo, status;
        int leftSpeed, rightSpeed;
        

        public MainPage()
        {
            this.InitializeComponent();
        }


        public async void StartControl() {
            
            bool drive = true;

            //FA_DLL.FA_ComOpen(portNo);

            while (drive == true)
            {

                await GetPadInput();

                try
                {
                    leftSpeed = (int)Math.Round(left * 100);
                    rightSpeed = (int)Math.Round(right * 100);
                }
                catch
                {

                }


                if (buttonPressed == "Y")
                {
                    drive = false;
                    startButton.Content = $"Start";
                }

                leftTrigger_text.Text = left.ToString();
                rightTrigger_text.Text = right.ToString();
                leftMotor_text.Text = leftSpeed.ToString();
                rightMotor_text.Text = rightSpeed.ToString();

                await Drive();
               
            }
            

            
        }


        private async Task Drive()
        {
            try
            {
                FA_DLL.FA_ComOpen(portNo);
                status = FA_DLL.FA_ComOpen(portNo);
                COM_status.Text= status.ToString();
            }
            catch
            {
                status = 'e';
                COM_status.Text = status.ToString();
            }

            if (status == '0')
            {


                FA_DLL.FA_SetMotors(portNo, leftSpeed, rightSpeed);
            }

            await Task.Delay(10);

        }

        private async Task GetPadInput()
        {
            GamepadReading pad = Gamepad.Gamepads.FirstOrDefault().GetCurrentReading();
            right = pad.RightTrigger;
            left = pad.LeftTrigger;

            buttonPressed = pad.Buttons.ToString();

            await Task.Delay(50);
        }

        private void COM_choose_LostFocus(object sender, RoutedEventArgs e)
        {
            portNo = char.Parse(COM_choose.Text);
        }

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {

            MessageDialog controllerFound = new MessageDialog("Controller connected?");
            bool goToDrive = false;


            if (Gamepad.Gamepads.Count > 0)
            {
                controllerFound.Commands.Add(new UICommand("Pad detected! Click to start!") { Id = 0 });
                goToDrive = true;
            }
            else
            {
                controllerFound.Commands.Add(new UICommand("No pad detected.. Try again!") { Id = 1 });
                goToDrive = false;
            }

            controllerFound.DefaultCommandIndex = 0;
            controllerFound.CancelCommandIndex = 1;

            var result = await controllerFound.ShowAsync();

            var btn = sender as Button;

            if (goToDrive == true)
            {
                btn.Content = $"Press Y to quit";
                StartControl();
               
            }
        }

    }
}
