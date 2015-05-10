﻿using System.Collections.Generic;
using Crayon.ParseTree;

namespace Crayon.Translator.CSharp
{
	class CSharpOpenTkSystemFunctionTranslator : CSharpSystemFunctionTranslator
	{
		protected override void TranslateBlitImage(List<string> output, Expression image, Expression x, Expression y)
		{
			output.Add("GameWindow.Instance.BlitImage((Image)");
			this.Translator.TranslateExpression(output, image);
			output.Add(", ");
			this.Translator.TranslateExpression(output, x);
			output.Add(", ");
			this.Translator.TranslateExpression(output, y);
			output.Add(")");
		}

		protected override void TranslateBlitImagePartial(List<string> output, Expression image, Expression targetX, Expression targetY, Expression targetWidth, Expression targetHeight, Expression sourceX, Expression sourceY, Expression sourceWidth, Expression sourceHeight)
		{
			output.Add("GameWindow.Instance.BlitImagePartial((Image)");
			this.Translator.TranslateExpression(output, image);
			output.Add(", ");
			this.Translator.TranslateExpression(output, targetX);
			output.Add(", ");
			this.Translator.TranslateExpression(output, targetY);
			output.Add(", ");
			this.Translator.TranslateExpression(output, targetWidth);
			output.Add(", ");
			this.Translator.TranslateExpression(output, targetHeight);
			output.Add(", ");
			this.Translator.TranslateExpression(output, sourceX);
			output.Add(", ");
			this.Translator.TranslateExpression(output, sourceY);
			output.Add(", ");
			this.Translator.TranslateExpression(output, sourceWidth);
			output.Add(", ");
			this.Translator.TranslateExpression(output, sourceHeight);
			output.Add(")");
		}

		protected override void TranslateDrawEllipse(List<string> output, Expression left, Expression top, Expression width, Expression height, Expression red, Expression green, Expression blue, Expression alpha)
		{
			output.Add("GameWindow.Instance.DrawEllipse(");
			this.Translator.TranslateExpression(output, left);
			output.Add(", ");
			this.Translator.TranslateExpression(output, top);
			output.Add(", ");
			this.Translator.TranslateExpression(output, width);
			output.Add(", ");
			this.Translator.TranslateExpression(output, height);
			output.Add(", ");
			this.Translator.TranslateExpression(output, red);
			output.Add(", ");
			this.Translator.TranslateExpression(output, green);
			output.Add(", ");
			this.Translator.TranslateExpression(output, blue);
			output.Add(", ");
			this.Translator.TranslateExpression(output, alpha);
			output.Add(")");
		}

		protected override void TranslateDrawLine(List<string> output, Expression ax, Expression ay, Expression bx, Expression by, Expression lineWidth, Expression red, Expression green, Expression blue, Expression alpha)
		{
			output.Add("GameWindow.Instance.DrawLine(");
			this.Translator.TranslateExpression(output, ax);
			output.Add(", ");
			this.Translator.TranslateExpression(output, ay);
			output.Add(", ");
			this.Translator.TranslateExpression(output, bx);
			output.Add(", ");
			this.Translator.TranslateExpression(output, by);
			output.Add(", ");
			this.Translator.TranslateExpression(output, lineWidth);
			output.Add(", ");
			this.Translator.TranslateExpression(output, red);
			output.Add(", ");
			this.Translator.TranslateExpression(output, green);
			output.Add(", ");
			this.Translator.TranslateExpression(output, blue);
			output.Add(", ");
			this.Translator.TranslateExpression(output, alpha);
			output.Add(")");
		}

		protected override void TranslateDrawRectangle(List<string> output, Expression left, Expression top, Expression width, Expression height, Expression red, Expression green, Expression blue, Expression alpha)
		{
			output.Add("GameWindow.Instance.DrawRectangle(");
			this.Translator.TranslateExpression(output, left);
			output.Add(", ");
			this.Translator.TranslateExpression(output, top);
			output.Add(", ");
			this.Translator.TranslateExpression(output, width);
			output.Add(", ");
			this.Translator.TranslateExpression(output, height);
			output.Add(", ");
			this.Translator.TranslateExpression(output, red);
			output.Add(", ");
			this.Translator.TranslateExpression(output, green);
			output.Add(", ");
			this.Translator.TranslateExpression(output, blue);
			output.Add(", ");
			this.Translator.TranslateExpression(output, alpha);
			output.Add(")");
		}

		protected override void TranslateFillScreen(List<string> output, Expression red, Expression green, Expression blue)
		{
			output.Add("GameWindow.Instance.FillScreen(");
			this.Translator.TranslateExpression(output, red);
			output.Add(", ");
			this.Translator.TranslateExpression(output, green);
			output.Add(", ");
			this.Translator.TranslateExpression(output, blue);
			output.Add(")");
		}

		protected override void TranslateGamepadEnableDevice(List<string> output, Expression device)
		{
			output.Add("GameWindow.Instance.GamepadEnableDevice((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(")");
		}

		protected override void TranslateGamepadGetAxisCount(List<string> output, Expression device)
		{
			output.Add("GameWindow.Instance.GetGamepadAxisCount((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(")");
		}

		protected override void TranslateGamepadGetAxisValue(List<string> output, Expression device, Expression axisIndex)
		{
			output.Add("GameWindow.Instance.GetGamepadAxisValue((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(", ");
			this.Translator.TranslateExpression(output, axisIndex);
			output.Add(")");
		}
		
		protected override void TranslateGamepadGetButtonCount(List<string> output, Expression device)
		{
			output.Add("GameWindow.Instance.GetGamepadButtonCount((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(")");
		}

		protected override void TranslateGamepadGetDeviceCount(List<string> output)
		{
			output.Add("GameWindow.Instance.GetGamepadCount()");
		}

		protected override void TranslateGamepadGetDeviceName(List<string> output, Expression device)
		{
			output.Add("GameWindow.Instance.GetGamepadDeviceName((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(")");
		}

		protected override void TranslateGamepadGetRawDevice(List<string> output, Expression index)
		{
			output.Add("GameWindow.Instance.GetGamepadRawDevice(");
			this.Translator.TranslateExpression(output, index);
			output.Add(")");
		}

		protected override void TranslateGamepadGetHatCount(List<string> output, Expression device)
		{
			output.Add("GameWindow.Instance.GetGamepadHatCount((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(")");
		}

		protected override void TranslateGamepadIsButtonPressed(List<string> output, Expression device, Expression buttonIndex)
		{
			output.Add("GameWindow.Instance.IsGamepadButtonPushed((OpenTK.Input.JoystickDevice)");
			this.Translator.TranslateExpression(output, device);
			output.Add(", ");
			this.Translator.TranslateExpression(output, buttonIndex);
			output.Add(")");
		}

		protected override void TranslateGlLoadTexture(List<string> output, Expression platformBitmapResource)
		{
			output.Add("GlUtil.ForceLoadTexture((System.Drawing.Bitmap)");
			this.Translator.TranslateExpression(output, platformBitmapResource);
			output.Add(")");
		}

		protected override void TranslateGlMaxTextureSize(List<string> output)
		{
			output.Add("GlUtil.MaxTextureSize");
		}
	}
}
