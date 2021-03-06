﻿// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa
{
    public class ButtonConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);

            return controlType == NativeControlType.Button ||
                   controlType == NativeControlType.ButtonHelp;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var button = new NSButton();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case NativeControlType.Button:
                    button.BezelStyle = NSBezelStyle.Rounded;
                    break;
                case NativeControlType.ButtonHelp:
                    button.BezelStyle = NSBezelStyle.HelpButton;
                    button.Title = string.Empty;
                    break;
            }

            button.ControlSize = CocoaHelpers.GetNSControlSize(controlVariant);
            button.Font = CocoaHelpers.GetNSFont(controlVariant);

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                FigmaText text = group.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);

                if (text != null && controlType != NativeControlType.ButtonHelp)
                    button.Title = text.characters;

                if (group.name == ComponentString.STATE_DISABLED)
                    button.Enabled = false;

                if (group.name == ComponentString.STATE_DEFAULT)
                    button.KeyEquivalent = "\r";
            }

            return new View(button);
        }


        protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out NativeControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor (name, GetControlType(currentNode.Node).FullName, rendererService.NodeRendersVar (currentNode, parentNode));

            switch (controlType)
            {
                case NativeControlType.Button:
                    code.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);
                    break;
                case NativeControlType.ButtonHelp:
                    code.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.HelpButton);
                    code.WriteEquality(name, nameof(NSButton.Title), string.Empty, inQuotes: true);
                    break;
            }

            code.WriteEquality(name, nameof(NSButton.ControlSize), CocoaHelpers.GetNSControlSize(controlVariant));
            code.WriteEquality(name, nameof(NSSegmentedControl.Font), CocoaCodeHelpers.GetNSFontString(controlVariant));

            FigmaGroup group = frame.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault(s => s.visible);

            if (group != null) {
                FigmaText text = group.children
                    .OfType<FigmaText> ()
                    .FirstOrDefault (s => s.name == ComponentString.TITLE);

                if (text != null && controlType != NativeControlType.ButtonHelp)
                {
                    string labelTranslated = NativeControlHelper.GetTranslatableString(text.characters, rendererService.CurrentRendererOptions.TranslateLabels);
                    code.WriteEquality(name, nameof(NSButton.Title), labelTranslated, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
                }

                if (group.name == ComponentString.STATE_DISABLED)
                    code.WriteEquality(name, nameof(NSButton.Enabled), false);

                if (group.name == ComponentString.STATE_DEFAULT)
                    code.WriteEquality (name, nameof(NSButton.KeyEquivalent), "\\r", true);
            }

            return code;
        }
    }
}
