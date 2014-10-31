using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GdiPresentation.Demo
{
    [DisplayName("Lots of text")]
    internal class LargeAmountOfTextDemo : ElementDemo
    {
        public override System.Windows.Forms.ScrollBars AllowedScrollBars
        {
            get { return System.Windows.Forms.ScrollBars.Vertical; }
        }

        public override Element BuildContent()
        {
            var result = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            for (int i = 0; i < 100; i++)
            {
                result.Children.Add(new TextBlock
                {
                    Wrap = true,
                    Margin = new Thickness(5),
                    Runs =
                    {
                        new Run { Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam et risus dictum risus fermentum bibendum. Suspendisse potenti. In hac habitasse platea dictumst. Phasellus eget scelerisque mauris. Donec a purus neque, nec elementum quam. Ut volutpat elementum cursus. Donec egestas sapien a diam eleifend suscipit. Curabitur sed nunc ligula. Cras et nulla sed ipsum dictum congue. Etiam aliquam, magna sit amet viverra venenatis, lorem ipsum facilisis elit, posuere pharetra erat mi ac velit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Integer ut quam a nunc luctus aliquet. Nulla facilisi. Aliquam erat volutpat. Quisque lacus sapien, mattis non dictum mollis, faucibus vel sapien." }
                    }
                });
            }

            return result;
        }
    }
}
