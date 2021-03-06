﻿/* 
* FigmaTextConverter.cs
* 
* Author:
*   Jose Medrano <josmed@microsoft.com>
*
* Copyright (C) 2018 Microsoft, Corp
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to permit
* persons to whom the Software is furnished to do so, subject to the
* following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
* NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
* DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
* OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
* USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Text;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaTextConverter : FigmaTextConverterBase
    {
        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaText = ((FigmaText)currentNode);
            Console.WriteLine("'{0}' with Font:'{1}({2})' s:{3} w:{4} ...", figmaText.characters, figmaText.style.fontFamily, figmaText.style.fontPostScriptName, figmaText.style.fontSize, figmaText.style.fontWeight);
            var label = new Label();
			var textField = label.NativeObject as FNSTextField;
			textField.Font = figmaText.style.ToNSFont();
			label.Text = figmaText.characters;
            textField.Configure(figmaText);
            textField.Configure (currentNode);
            return label;
        }

        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var figmaText = (FigmaText)currentNode.Node;

            StringBuilder builder = new StringBuilder();
            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                builder.WriteEquality (currentNode.Name, null, FigmaExtensions.CreateLabelToDesignerString (figmaText.characters), instanciate: true);

            //builder.Configure(figmaText, currentNode.Name);
            builder.Configure (currentNode.Node, currentNode.Name);

            var alignment = FigmaExtensions.ToNSTextAlignment (figmaText.style.textAlignHorizontal);
			if (alignment != default) {
                builder.WriteEquality (currentNode.Name, nameof (AppKit.NSTextField.Alignment), alignment);
            }
            return builder.ToString();
        }

        public override Type GetControlType(FigmaNode currentNode)
            => typeof (AppKit.NSTextField);
    }
}
