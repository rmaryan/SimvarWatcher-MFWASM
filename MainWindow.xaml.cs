using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Simvars
{
    interface IBaseSimConnectWrapper
    {
        int GetUserSimConnectWinEvent();
        void ReceiveSimConnectMessage();
        void SetWindowHandle(IntPtr _hWnd);
        void Disconnect();
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new SimvarsViewModel();

            InitializeComponent();
            cbb_UnitNames.SelectedItem = null;
        }

        protected HwndSource GetHWinSource()
        {
            return PresentationSource.FromVisual(this) as HwndSource;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            GetHWinSource().AddHook(WndProc);
            if (this.DataContext is IBaseSimConnectWrapper oBaseSimConnectWrapper)
            {
                oBaseSimConnectWrapper.SetWindowHandle(GetHWinSource().Handle);
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            if (this.DataContext is IBaseSimConnectWrapper oBaseSimConnectWrapper)
            {
                try
                {
                    if (iMsg == oBaseSimConnectWrapper.GetUserSimConnectWinEvent())
                    {
                        oBaseSimConnectWrapper.ReceiveSimConnectMessage();
                    }
                }
                catch
                {
                    oBaseSimConnectWrapper.Disconnect();
                }
            }

            return IntPtr.Zero;
        }

        private void LinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            string sText = e.Text;
            foreach (char c in sText)
            {
                if (!(('0' <= c && c <= '9') || c == '+' || c == '-' || c == ','))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private bool isSliderDrag = false;
        private void updateSliderValue(object sender)
        {
            if (sender is Slider oSlider && this.DataContext is SimvarsViewModel oContext)
            {
                oContext.SetTickSliderValue((int)oSlider.Value);
            }
        }

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.updateSliderValue(sender);
            this.isSliderDrag = false;
        }

        private void Slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.isSliderDrag = true;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.isSliderDrag)
            {
                this.updateSliderValue(sender);
            }
        }

        private void TextBox_SearchBar_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is SimvarsViewModel oContext)
            {
                string[] sSearchInput = (tb_SearchBar.Text.ToUpper()).Split(' ');

                IEnumerable<string> lSimvarsNamesFiltered = new List<string>();
                foreach (string searchWord in sSearchInput)
                {
                    if (lSimvarsNamesFiltered.Count() > 0)
                    {
                        lSimvarsNamesFiltered = lSimvarsNamesFiltered.Intersect(oContext.aSimvarNames.Where(
                            stringToCheck => stringToCheck.Contains(searchWord)
                        ));
                    }
                    else
                    {
                        lSimvarsNamesFiltered = oContext.aSimvarNames.Where(
                            stringToCheck => stringToCheck.Contains(searchWord)
                        );
                    }
                }
                if(!Enumerable.SequenceEqual(oContext.aSimvarNamesFiltered, lSimvarsNamesFiltered))
                {
                    oContext.aSimvarNamesFiltered = lSimvarsNamesFiltered.ToArray();
                }
            }
        }

        private void ComboBox_SimvarNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.updateUnits();
        }

        private void updateUnits(string selectedUnit = "")
        {
            if (this.DataContext is SimvarsViewModel oContext && cbb_UnitNames != null)
            {
                int selectedSimVarIndex = Array.IndexOf(oContext.aSimvarNames, oContext.sSimvarRequest);
                if (selectedSimVarIndex >= 0)
                {
                    string defaultUnit = SimUtils.SimVars.DefaultUnits[selectedSimVarIndex].ToLower();
                    string[] compatibleUnits = Array.Find(SimUtils.Units.Mapping, array => array.Contains(defaultUnit, StringComparer.OrdinalIgnoreCase));
                    if (compatibleUnits != null && !oContext.bShowAllUnits)
                    {
                        Array.Sort(compatibleUnits);
                        oContext.aUnitNamesFiltered = compatibleUnits;
                    }
                    else
                    {
                        oContext.aUnitNamesFiltered = oContext.aUnitNames;
                    }

                    if (String.IsNullOrEmpty(selectedUnit) || !oContext.aUnitNamesFiltered.Contains(selectedUnit, StringComparer.OrdinalIgnoreCase))
                    {
                        selectedUnit = defaultUnit;
                    }
                    
                    int unitIndex = Array.FindIndex(oContext.aUnitNamesFiltered, unitToSearch => String.Equals(unitToSearch, selectedUnit, StringComparison.OrdinalIgnoreCase));
                    if (unitIndex >= 0)
                    {
                        cbb_UnitNames.SelectedIndex = unitIndex;
                    }
                    else
                    {
                        cbb_UnitNames.SelectedIndex = -1;
                    }

                    oContext.bIsString = selectedUnit.ToLower().StartsWith("string");
                }
            }
        }

        private void ComboBox_UnitNames_Loaded(object sender, RoutedEventArgs e)
        {
            this.updateUnits();
        }

        private void ShowAllUnits_CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            string selectedUnit = "";
            if(cbb_UnitNames.SelectedIndex != -1)
            {
                selectedUnit = cbb_UnitNames.SelectedValue.ToString();
            }
            this.updateUnits(selectedUnit);
        }
        private void openSimConnectReferenceLink(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/SimConnect_SDK.htm");
        }

        private void openSimulationVariablesReference(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.flightsimulator.com/html/Programming_Tools/SimVars/Simulation_Variables.htm");
        }

        private void clickClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
