using System;
using System.IO;
using Sandbox.ModAPI;

namespace AwwScrap_IFoundYourCrap.Thraxus.Common.Utilities.FileHandlers
{
	public static class Save
	{
		public static void WriteBinaryFileToWorldStorage<T>(string fileName, T data, Type type)
		{
			if (MyAPIGateway.Utilities.FileExistsInWorldStorage(fileName, type))
				MyAPIGateway.Utilities.DeleteFileInWorldStorage(fileName, type);

			using (BinaryWriter binaryWriter = MyAPIGateway.Utilities.WriteBinaryFileInWorldStorage(fileName, type))
			{
				if (binaryWriter == null)
					return;
				byte[] binary = MyAPIGateway.Utilities.SerializeToBinary(data);
				binaryWriter.Write(binary.Length);
				binaryWriter.Write(binary);
				binaryWriter.Flush();
			}
		}

		public static void WriteXmlFileToWorldStorage<T>(string fileName, T data, Type type)
		{
			if (MyAPIGateway.Utilities.FileExistsInWorldStorage(fileName, type))
				MyAPIGateway.Utilities.DeleteFileInWorldStorage(fileName, type);

			using (TextWriter textWriter = MyAPIGateway.Utilities.WriteFileInWorldStorage(fileName, type))
			{
				if (textWriter == null)
					return;
				string text = MyAPIGateway.Utilities.SerializeToXML(data);
				textWriter.Write(text.Length);
				textWriter.Write(text);
				textWriter.Flush();
			}
		}

		public static void WriteTextFileToWorldStorage<T>(string fileName, T data, Type type)
		{
			if (MyAPIGateway.Utilities.FileExistsInWorldStorage(fileName, type))
				MyAPIGateway.Utilities.DeleteFileInWorldStorage(fileName, type);

			using (TextWriter textWriter = MyAPIGateway.Utilities.WriteFileInWorldStorage(fileName, type))
			{
				if (textWriter == null)
					return;
				textWriter.Write(data);
				textWriter.Flush();
			}
		}

		public static void WriteToSandbox(Type T)
		{

		}
	}
}
