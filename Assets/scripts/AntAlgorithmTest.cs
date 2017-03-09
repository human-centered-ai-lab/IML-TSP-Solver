using UnityEngine;
using UnityEngine.PlaymodeTests;
using UnityEngine.Assertions;
using System.Collections;

[EditModeTest]
public class AntAlgorithmTest {

	[EditModeTest]
	public void AntAlgorithmTestSimplePasses() {
		// Use the Assert class to test conditions.
	}

	[EditModeTest]
	public IEnumerator AntAlgorithmTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
