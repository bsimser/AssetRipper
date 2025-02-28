using AssetRipper.Core.Classes;
using AssetRipper.Core.Interfaces;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.Project;
using AssetRipper.Core.Project.Collections;
using AssetRipper.Core.Utils;
using AssetRipper.Library.Configuration;
using System.IO;
using System.Text.Json;

namespace AssetRipper.Library.Exporters.Miscellaneous
{
	public sealed class TextAssetExporter : BinaryAssetExporter
	{
		private TextExportMode exportMode;
		public TextAssetExporter(LibraryConfiguration configuration)
		{
			exportMode = configuration.TextExportMode;
		}

		public override bool IsHandle(IUnityObjectBase asset)
		{
			if (asset is ITextAsset textAsset)
				return IsValidData(textAsset.RawData);
			else
				return false;
		}

		public override IExportCollection CreateCollection(VirtualSerializedFile virtualFile, IUnityObjectBase asset)
		{
			return new AssetExportCollection(this, asset, GetExportExtension(asset));
		}

		public override bool Export(IExportContainer container, IUnityObjectBase asset, string path)
		{
			File.WriteAllBytes(path, ((ITextAsset)asset).RawData);
			return true;
		}

		private string GetExportExtension(IUnityObjectBase asset)
		{
			switch (exportMode)
			{
				case TextExportMode.Txt:
					return "txt";
				case TextExportMode.Parse:
					return GetExtension((ITextAsset)asset);
				case TextExportMode.Bytes:
				default:
					return "bytes";
			}
		}

		private static string GetExtension(ITextAsset asset)
		{
			if (IsValidJson(asset.Text))
				return "json";
			else
				return "txt";
		}

		private static bool IsValidJson(string text)
		{
			try
			{
				using (var parsed = JsonDocument.Parse(text))
				{
					return parsed != null;
				}
			}
			catch { }
			return false;
		}
	}
}
