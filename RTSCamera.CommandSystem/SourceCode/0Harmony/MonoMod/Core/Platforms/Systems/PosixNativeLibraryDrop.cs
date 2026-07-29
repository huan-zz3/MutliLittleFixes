using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	// Token: 0x02000522 RID: 1314
	internal abstract class PosixNativeLibraryDrop
	{
		// Token: 0x06001D80 RID: 7552
		[return: NativeInteger]
		protected abstract IntPtr Mkstemp(Span<byte> template);

		// Token: 0x06001D81 RID: 7553
		protected abstract void CloseFileDescriptor([NativeInteger] IntPtr fd);

		// Token: 0x06001D82 RID: 7554 RVA: 0x0005F424 File Offset: 0x0005D624
		[NullableContext(1)]
		public unsafe string DropLibrary(Stream sourceStream, [Nullable(0)] ReadOnlySpan<byte> defaultTemplate)
		{
			object obj;
			byte[] array;
			int num3;
			if (Switches.TryGetSwitchValue("HelperDropPath", out obj))
			{
				string text = obj as string;
				if (text != null)
				{
					int num = defaultTemplate.LastIndexOf(47);
					Helpers.Assert(num >= 0, null, "endOfDefaultTemplateDir >= 0");
					ReadOnlySpan<byte> readOnlySpan = defaultTemplate.Slice(num);
					text = Path.GetFullPath(text);
					Directory.CreateDirectory(text);
					int byteCount = Encoding.UTF8.GetByteCount(text);
					array = ArrayPool<byte>.Shared.Rent(byteCount + readOnlySpan.Length + 1);
					array.AsSpan<byte>().Clear();
					int num2;
					fixed (char* pinnableReference = text.AsSpan().GetPinnableReference())
					{
						char* ptr = pinnableReference;
						byte[] array2;
						byte* ptr2;
						if ((array2 = array) == null || array2.Length == 0)
						{
							ptr2 = null;
						}
						else
						{
							ptr2 = &array2[0];
						}
						num2 = Encoding.UTF8.GetBytes(ptr, text.Length, ptr2, array.Length);
						array2 = null;
					}
					if (array[num2 - 1] == 47)
					{
						num2--;
					}
					readOnlySpan.CopyTo(array.AsSpan<byte>(num2));
					array[num2 + readOnlySpan.Length] = 0;
					num3 = num2 + readOnlySpan.Length;
					goto IL_014B;
				}
			}
			array = ArrayPool<byte>.Shared.Rent(defaultTemplate.Length + 1);
			array.AsSpan<byte>().Clear();
			defaultTemplate.CopyTo(array);
			num3 = defaultTemplate.Length;
			IL_014B:
			IntPtr intPtr = this.Mkstemp(array);
			string @string = Encoding.UTF8.GetString(array, 0, num3);
			ArrayPool<byte>.Shared.Return(array, false);
			if (PlatformDetection.Runtime == RuntimeKind.Mono && PlatformDetection.Corelib != CorelibKind.Core)
			{
				this.CloseFileDescriptor(intPtr);
				using (FileStream fileStream = new FileStream(@string, FileMode.Create, FileAccess.Write))
				{
					sourceStream.CopyTo(fileStream);
					return @string;
				}
			}
			try
			{
				using (FileStream fileStream2 = new FileStream(intPtr, FileAccess.Write))
				{
					sourceStream.CopyTo(fileStream2);
				}
			}
			finally
			{
				this.CloseFileDescriptor(intPtr);
			}
			return @string;
		}
	}
}
