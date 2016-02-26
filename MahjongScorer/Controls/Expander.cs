using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

namespace MahjongScorer.Controls
{
    [TemplateVisualState(Name = StringConstants.StateContentExpanded, GroupName = StringConstants.GroupContent)]
    [TemplateVisualState(Name = StringConstants.StateContentCollapsed, GroupName = StringConstants.GroupContent)]
    [TemplatePart(Name = StringConstants.ExpanderToggleButtonPart, Type = typeof (ToggleButton))]
    [TemplatePart(Name = StringConstants.HeaderButtonPart, Type = typeof (ButtonBase))]
    [ContentProperty(Name = "Content")]
    public sealed class Expander : Control
    {
        internal static readonly DependencyProperty ExpanderToggleButtonStyleProperty =
            DependencyProperty.Register("ExpanderToggleButtonStyle", typeof (Style), typeof (Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty HeaderButtonStyleProperty =
            DependencyProperty.Register("HeaderButtonStyle", typeof (Style), typeof (Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty HeaderButtonContentProperty =
            DependencyProperty.Register("HeaderButtonContent", typeof (object), typeof (Expander),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof (object), typeof (Expander), new PropertyMetadata(null));


        internal static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof (bool), typeof (Expander), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = d as Expander;
            if (expander == null || expander._expanderButton == null)
                return;
            var isExpanded = (bool)e.NewValue;
            expander._expanderButton.IsChecked = isExpanded;
            if (isExpanded)
                expander.ExpandControl();
            else
                expander.CollapseControl();
        }


        private ToggleButton _expanderButton;
        private ButtonBase _headerButton;

        private RowDefinition _mainContentRow;

        public Expander()
        {
            DefaultStyleKey = typeof (Expander);
        }

        public bool IsExpanded
        {
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public object HeaderButtonContent
        {
            get { return GetValue(HeaderButtonContentProperty); }
            set { SetValue(HeaderButtonContentProperty, value); }
        }

        public Style HeaderButtonStyle
        {
            get { return (Style) GetValue(HeaderButtonStyleProperty); }
            set { SetValue(HeaderButtonStyleProperty, value); }
        }

        public Style ExpanderToggleButtonStyle
        {
            get { return (Style) GetValue(ExpanderToggleButtonStyleProperty); }
            set { SetValue(ExpanderToggleButtonStyleProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            _expanderButton = GetTemplateChild(StringConstants.ExpanderToggleButtonPart) as ToggleButton;
            _headerButton = GetTemplateChild(StringConstants.HeaderButtonPart) as ButtonBase;
            _mainContentRow = GetTemplateChild(StringConstants.MainContentRowPart) as RowDefinition;

            if (_expanderButton != null)
            {
                _expanderButton.Checked += OnExpanderButtonChecked;
                _expanderButton.Unchecked += OnExpanderButtonUnChecked;
                _expanderButton.IsChecked = IsExpanded;
                if (IsExpanded)
                    ExpandControl();
                else
                    CollapseControl();
            }

            if (_headerButton != null)
            {
                _headerButton.Click += OnHeaderButtonClick;
            }
        }

        private void OnHeaderButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsExpanded == true)
            {
                IsExpanded = false;
                CollapseControl();
            }
            else
            {
                IsExpanded = true;
                ExpandControl();
            }
        }

        private void ExpandControl()
        {
            if (_mainContentRow == null || !_mainContentRow.Height.Value.Equals(0d))
                return;
            VisualStateManager.GoToState(this, StringConstants.StateContentExpanded, true);
            _mainContentRow.Height = new GridLength(1, GridUnitType.Auto);
        }

        private void CollapseControl()
        {
            if (_mainContentRow == null || _mainContentRow.Height.Value.Equals(0d))
                return;
            VisualStateManager.GoToState(this, StringConstants.StateContentCollapsed, true);
            _mainContentRow.Height = new GridLength(0d);
        }

        private void OnExpanderButtonUnChecked(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
            CollapseControl();
        }

        private void OnExpanderButtonChecked(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
            ExpandControl();
        }
    }
}