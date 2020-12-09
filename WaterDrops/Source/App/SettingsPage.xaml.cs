﻿using System;
using System.Reflection;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WaterDrops
{
    public sealed partial class SettingsPage : Page
    {
        private Settings settings;

        public SettingsPage()
        {
            this.InitializeComponent();

            // Try to cache the page, if the cache size allows it
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            this.Loaded += (sender, e) =>
            {
                // Initialize settings
                StartupToggle.IsEnabled = settings.CanToggleAutoStartup;
                StartupToggle.IsOn = settings.AutoStartupEnabled;
                StartupDescriptionTextBlock.Text = settings.AutoStartupStateDescription;

                // Retrieve application information from the current assembly using AssemblyInfo
                Assembly assembly = Assembly.GetExecutingAssembly();
                AppTitleLabel.Text = AssemblyInfo.GetAttribute<AssemblyTitleAttribute>(assembly).Title;
                CopyrightLabel.Text = AssemblyInfo.GetAttribute<AssemblyCopyrightAttribute>(assembly).Copyright;
                AuthorLabel.Text = AssemblyInfo.GetAttribute<AssemblyCompanyAttribute>(assembly).Company;
                AppVersionLabel.Text = AssemblyInfo.GetAttribute<AssemblyFileVersionAttribute>(assembly).Version;
                AppReleaseLabel.Text = AssemblyInfo.GetAttribute<AssemblyDescriptionAttribute>(assembly).Description;

                // Register callbacks for updating UI elements when some settings are changed by the code
                settings.AutoStartupSettingChanged += UpdateStartupSettingToggle;
            };

            this.Unloaded += (sender, e) =>
            {
                // Detach event handlers
                settings.AutoStartupSettingChanged -= UpdateStartupSettingToggle;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Catch the parameter that has been forwarded from the MainPage
            this.settings = (Settings)e.Parameter;
        }

        private void StartupSetting_Toggled(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
                return;

            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            // Update setting only if it has changed from the previous value
            if (settings.CanToggleAutoStartup)
                settings.TryChangeAutoStartupSetting(toggleSwitch.IsOn);
        }

        private void UpdateStartupSettingToggle(bool autoStartupEnabled, EventArgs args)
        {
            StartupToggle.IsOn = autoStartupEnabled;
            StartupToggle.IsEnabled = settings.CanToggleAutoStartup;
            StartupDescriptionTextBlock.Text = settings.AutoStartupStateDescription;
        }
    }
}
