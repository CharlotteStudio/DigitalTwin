using UnityEngine;

/// <summary>
/// it must push outside of Editor folder
/// </summary>
public class CustomAttribute : MonoBehaviour { }

public class ReadOnlyAttribute : PropertyAttribute { }

public class ShowOnlyAttribute : PropertyAttribute { }

public class BeginReadOnlyGroupAttribute : PropertyAttribute { }

public class EndReadOnlyGroupAttribute : PropertyAttribute { }