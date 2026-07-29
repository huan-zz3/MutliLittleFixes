using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HarmonyLib
{
	// Token: 0x020000A5 RID: 165
	internal static class PatchInfoSerialization
	{
		// Token: 0x0600034B RID: 843 RVA: 0x00011B88 File Offset: 0x0000FD88
		internal static byte[] Serialize(this PatchInfo patchInfo)
		{
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PatchInfoSerialization.binaryFormatter.Serialize(memoryStream, patchInfo);
				array = memoryStream.ToArray();
			}
			return array;
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00011BCC File Offset: 0x0000FDCC
		internal static PatchInfo Deserialize(byte[] bytes)
		{
			PatchInfo patchInfo;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				patchInfo = (PatchInfo)PatchInfoSerialization.binaryFormatter.Deserialize(memoryStream);
			}
			return patchInfo;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00011C10 File Offset: 0x0000FE10
		internal static int PriorityComparer(object obj, int index, int priority)
		{
			Traverse traverse = Traverse.Create(obj);
			int value = traverse.Field("priority").GetValue<int>();
			int value2 = traverse.Field("index").GetValue<int>();
			if (priority != value)
			{
				return -priority.CompareTo(value);
			}
			return index.CompareTo(value2);
		}

		// Token: 0x0400023B RID: 571
		internal static readonly BinaryFormatter binaryFormatter = new BinaryFormatter
		{
			Binder = new PatchInfoSerialization.Binder()
		};

		// Token: 0x020000A6 RID: 166
		private class Binder : SerializationBinder
		{
			// Token: 0x0600034F RID: 847 RVA: 0x00011C74 File Offset: 0x0000FE74
			public override Type BindToType(string assemblyName, string typeName)
			{
				Type[] array = new Type[]
				{
					typeof(PatchInfo),
					typeof(Patch[]),
					typeof(Patch)
				};
				foreach (Type type in array)
				{
					if (typeName == type.FullName)
					{
						return type;
					}
				}
				return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
			}
		}
	}
}
