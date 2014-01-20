namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Properties;

    public class VisualStateHelper : DependencyObject
    {
        public static readonly DependencyProperty VisualStateNameProperty = DependencyProperty.RegisterAttached(
            "VisualStateName",
            typeof(string),
            typeof(VisualStateHelper),
            new PropertyMetadata(OnVisualStateNameChanged));

        public static string GetVisualStateName(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullResourceException("target", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            return target.GetValue(VisualStateNameProperty).ToString();
        }

        public static void SetVisualStateName(DependencyObject target, string visualStateName)
        {
            // This may throw an exception if an enum is used. However, it shouldn't be used as the user control
            // should not set its own state, the controller will always set it.
            if (target == null)
            {
                throw new ArgumentNullResourceException("target", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            try
            {
                target.SetValue(VisualStateNameProperty, visualStateName);
            } catch (Exception ex)
            {
                throw new NotSupportedException(Resources.VisualStateHelper_SetVisualStateName_Setting_visual_states_from_within_the_user_control, ex);
            }
        }

        private static void OnVisualStateNameChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
            {
                return;
            }

            string visualStateName = args.NewValue.ToString();

            var control = sender as Control; // Must be a control, ie an input control.
            if (control == null)
            {
                throw new InvalidOperationException(Resources.VisualStateHelper_OnVisualStateNameChanged_This_attached_property_only_supports_types_derived_from_Control);
            }

            // Apply the visual state.
            VisualStateManager.GoToState(control, visualStateName, true);
        }
    }
}