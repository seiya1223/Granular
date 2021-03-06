﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public class UserControl : ContentControl
    {
        static UserControl()
        {
            Control.IsTabStopProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(false));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
            VerticalContentAlignmentProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
            FocusableProperty.OverrideMetadata(typeof(UserControl), new FrameworkPropertyMetadata(false));
        }
    }
}
