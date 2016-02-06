﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crayon.ParseTree;

namespace Crayon.Translator.CSharp
{
	internal class CSharpOpenTkOpenGlTranslator : AbstractOpenGlTranslator
	{
		public override bool IsNewStyle { get { return false; } }

		public override void TranslateGlBeginPolygon(List<string> output, Expression gl)
		{
			output.Add("GL.Begin(BeginMode.Polygon)");
		}

		public override void TranslateGlBeginQuads(List<string> output, Expression gl)
		{
			output.Add("GL.Begin(BeginMode.Quads)");
		}

		public override void TranslateGlBindTexture(List<string> output, Expression gl, Expression textureId)
		{
			output.Add("GL.BindTexture(TextureTarget.Texture2D, ");
			this.Translator.TranslateExpression(output, textureId);
			output.Add(")");
		}

		public override void TranslateGlColor4(List<string> output, Expression gl, Expression r, Expression g, Expression b, Expression a)
		{
			output.Add("GL.Color4((byte)(");
			this.Translator.TranslateExpression(output, r);
			output.Add("), (byte)(");
			this.Translator.TranslateExpression(output, g);
			output.Add("), (byte)(");
			this.Translator.TranslateExpression(output, b);
			output.Add("), (byte)(");
			this.Translator.TranslateExpression(output, a);
			output.Add("))");
		}

		public override void TranslateGlDisableTexture2D(List<string> output, Expression gl)
		{
			output.Add("GL.Disable(EnableCap.Texture2D)");
		}

		public override void TranslateGlDisableTexCoordArray(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlDisableVertexArray(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlDrawArrays(List<string> output, Expression gl, Expression vertexCount)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlDrawEllipseVertices(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlEnableTexture2D(List<string> output, Expression gl)
		{
			output.Add("GL.Enable(EnableCap.Texture2D)");
		}

		public override void TranslateGlEnableTextureCoordArray(List<string> output, Expression glReference)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlEnableVertexArray(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlEnd(List<string> output, Expression gl)
		{
			output.Add("GL.End()");
		}

		public override void TranslateGlFrontFaceCw(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlGetQuadTextureVbo(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlGetQuadVbo(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlLoadIdentity(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlLoadTexture(List<string> output, Expression gl, Expression platformBitmapResource)
		{
			output.Add("GlUtil.ForceLoadTexture((System.Drawing.Bitmap)");
			this.Translator.TranslateExpression(output, platformBitmapResource);
			output.Add(")");
		}

		public override void TranslateGlMaxTextureSize(List<string> output)
		{
			output.Add("GlUtil.MaxTextureSize");
		}

		public override void TranslateGlScale(List<string> output, Expression gl, Expression xratio, Expression yratio)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlTexCoord2(List<string> output, Expression gl, Expression x, Expression y)
		{
			output.Add("GL.TexCoord2(");
			this.Translator.TranslateExpression(output, x);
			output.Add(", ");
			this.Translator.TranslateExpression(output, y);
			output.Add(")");
		}

		public override void TranslateGlTexCoordPointer(List<string> output, Expression gl, Expression textureBuffer)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlTranslate(List<string> output, Expression gl, Expression dx, Expression dy)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlVertex2(List<string> output, Expression gl, Expression x, Expression y)
		{
			output.Add("GL.Vertex2(");
			this.Translator.TranslateExpression(output, x);
			output.Add(", ");
			this.Translator.TranslateExpression(output, y);
			output.Add(")");
		}

		public override void TranslateGlPrepareDrawPipeline(List<string> output, Expression gl)
		{
			throw new NotImplementedException();
		}

		public override void TranslateGlVertexPointer(List<string> output, Expression glReference, Expression vertexBuffer)
		{
			throw new NotImplementedException();
		}
	}
}