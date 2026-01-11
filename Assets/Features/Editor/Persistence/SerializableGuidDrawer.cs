#nullable enable

using System.Linq;
using Features;
using UnityEditor;

namespace Features.Persistence.Editor
{
	[CustomPropertyDrawer(typeof(SerializableGuid))]
	public sealed class SerializableGuidDrawer : PropertyDrawer
	{
		private static SerializedProperty[] GetGuidParts(SerializedProperty property) => new[] {
			property.FindPropertyRelative(nameof(SerializableGuid.Part1)),
			property.FindPropertyRelative(nameof(SerializableGuid.Part2)),
			property.FindPropertyRelative(nameof(SerializableGuid.Part3)),
			property.FindPropertyRelative(nameof(SerializableGuid.Part4)),
		};

		private static string BuildGuidString(SerializedProperty[] parts) => new System.Text.StringBuilder(capacity: 4)
			.AppendFormat(Helpers.HEX_FORMAT, parts[0].uintValue)
			.AppendFormat(Helpers.HEX_FORMAT, parts[1].uintValue)
			.AppendFormat(Helpers.HEX_FORMAT, parts[2].uintValue)
			.AppendFormat(Helpers.HEX_FORMAT, parts[3].uintValue)
			.ToString();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var guidParts = GetGuidParts(property);
			if (guidParts.All(Utils.NotNull))
			{
				EditorGUI.LabelField(position, BuildGuidString(guidParts));
			}
			else
			{
				EditorGUI.SelectableLabel(position, "GUID Not Initialized");
			}

			bool clicked = Event.current.type == EventType.MouseUp && Event.current.button == 1;
			if (clicked && position.Contains(Event.current.mousePosition))
			{
				ShowContextMenu(property, guidParts);
				Event.current.Use();
			}

			EditorGUI.EndProperty();
		}

		private void ShowContextMenu(SerializedProperty guid, SerializedProperty[] guidParts)
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Copy"), false, () => CopyGuid(guidParts));
			menu.AddItem(new GUIContent("Reset"), false, () => ResetGuid(guid, guidParts));
			menu.AddItem(new GUIContent("Regenerate"), false, () => RegenerateGuid(guid, guidParts));
			menu.ShowAsContext();
		}

		private void CopyGuid(SerializedProperty[] guidParts)
		{
			if (guidParts.Any(static p => p is null)) return;

			string guid = BuildGuidString(guidParts);
			EditorGUIUtility.systemCopyBuffer = guid;
			Debug.LogFormat("Copied GUID '{0}' to clipboard.", guid);
		}

		private void ResetGuid(SerializedProperty guid, SerializedProperty[] guidParts)
		{
			const string warning = "Are your sure you want to reset the GUID?";
			if (!EditorUtility.DisplayDialog("Reset GUID", warning, "Yes", "No")) return;

			foreach (var part in guidParts)
			{
				part.uintValue = 0;
			}

			guid.serializedObject.ApplyModifiedProperties();
			Debug.Log("GUID has been reset to default.");
		}

		private void RegenerateGuid(SerializedProperty guid, SerializedProperty[] guidParts)
		{
			const string warning = "Are your sure you want to regenerate the GUID?";
			if (!EditorUtility.DisplayDialog("Regenerate GUID", warning, "Yes", "No")) return;
			
			byte[] bytes = Guid.NewGuid().ToByteArray();

			for (int i = 0; i < guidParts.Length; i++)
			{
				guidParts[i].uintValue = BitConverter.ToUInt32(bytes, i * 4);
			}

			guid.serializedObject.ApplyModifiedProperties();
			Debug.Log("GUID has been regenerated.");
		}
	}
}
