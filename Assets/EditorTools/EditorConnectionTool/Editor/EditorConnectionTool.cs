using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine;

[EditorTool("ConnectionTool", typeof(ConnectionBlock))]
public class EditorConnectionTool : EditorTool
{
   private const float MaxConnectionDistance = 5f;
   
   [SerializeField] private Texture2D toolIcon;

   private Transform _prevTransform;
   private Connector[] _allConnectors;
   private Connector[] _currentConnectors;
   private ConnectionBlock _currentConnectionBlock;
   
   public override GUIContent toolbarIcon => new GUIContent()
      { image = toolIcon, text = "Connection Tool", tooltip = "Connection Tool" };

   public override void OnToolGUI(EditorWindow window)
   {
      _currentConnectionBlock = (ConnectionBlock)target;
      Transform currentTransform = _currentConnectionBlock.transform;
      
      if (currentTransform != _prevTransform)
      {
         PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(currentTransform.gameObject);

         _allConnectors = prefabStage == null ? FindObjectsOfType<Connector>() : prefabStage.prefabContentsRoot.GetComponentsInChildren<Connector>();
         
         _currentConnectors = currentTransform.GetComponentsInChildren<Connector>();
         _prevTransform = currentTransform;
      }
      
      EditorGUI.BeginChangeCheck();
      Vector3 targetPosition = Handles.PositionHandle(currentTransform.position, Quaternion.identity);

      if (EditorGUI.EndChangeCheck())
      {
         Undo.RecordObject(currentTransform, "Moving of connection block");
         TryConnect(currentTransform, targetPosition);
      }
   }

   private void TryConnect(Transform currentTransform, Vector3 targetPosition)
   {
      Vector3 closesPosition = targetPosition;
      float closesDistance = float.PositiveInfinity;

      foreach (var someConnector in _allConnectors)
      {
         if (someConnector.transform.GetComponentInParent<ConnectionBlock>() == _currentConnectionBlock) continue;

         foreach (var currentConnector in _currentConnectors)
         {
            Vector3 newTargetPosition = someConnector.transform.position - (currentConnector.transform.position - currentTransform.position);
            float newDistance = Vector3.Distance(newTargetPosition, targetPosition);

            if (newDistance < closesDistance)
            {
               closesPosition = newTargetPosition;
               closesDistance = newDistance;
            }
         }
      }

      currentTransform.position = closesDistance <= MaxConnectionDistance ? closesPosition : targetPosition;
   }
}
