using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXImportDefaultSetting : AssetPostprocessor
{


	void OnPreprocessModel()
	{
        if (".fbx" == Path.GetExtension(assetPath))
		{
			ModelImporter modelImporter = this.assetImporter as ModelImporter;

			if(modelImporter != null)
			{
				modelImporter.materialName = ModelImporterMaterialName.BasedOnModelNameAndMaterialName;
				Debug.Log("スクリプトによりFBXインポート設定を自動的に変更しました");
			}

        }
    }

}
