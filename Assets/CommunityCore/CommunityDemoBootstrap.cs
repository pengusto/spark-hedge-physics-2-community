using UnityEngine;

/// <summary>
/// Creates a tiny physics-only playground at runtime. It uses the upstream
/// CharacterPhysics component and deliberately contains no Spark character art.
/// </summary>
public sealed class CommunityDemoBootstrap : MonoBehaviour
{
    private void Awake()
    {
        CreateWorld();
        CreateGround();
        CreatePlayer();
        CreateCamera();
    }

    private static void CreateWorld()
    {
        if (World.WorldInfo != null) return;
        var world = new GameObject("World");
        world.AddComponent<World>();
    }

    private static void CreateGround()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Demo Ground";
        ground.transform.localScale = new Vector3(3f, 1f, 3f);
        ground.AddComponent<SurfaceMaterialInfo>();
    }

    private static void CreatePlayer()
    {
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Physics Character";
        player.transform.position = new Vector3(0f, 1.2f, 0f);

        var body = player.AddComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        var controller = player.AddComponent<CharacterPhysics>();
        controller.rigid = body;
        controller.PlatformReference = player.transform;
        controller.GroundRayMask = ~0;
        controller.CheckGroundTime = 0.1f;
        controller.EnableMovement = false;
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Demo Camera");
        var camera = cameraObject.AddComponent<Camera>();
        cameraObject.transform.position = new Vector3(5f, 4f, -7f);
        cameraObject.transform.LookAt(new Vector3(0f, 0.8f, 0f));
        camera.fieldOfView = 55f;
    }
}
