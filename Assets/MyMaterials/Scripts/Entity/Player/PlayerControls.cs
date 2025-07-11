//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/MyMaterials/Scripts/Entity/Player/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""710e4cfc-ca6a-47bb-93bd-0567875df012"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""40c14192-9084-4273-a909-ddb76c93bb1d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""0706a178-e404-4696-a0c0-ea06a7fdd44b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ThrustUp"",
                    ""type"": ""Button"",
                    ""id"": ""88339aa5-6708-40ae-a45d-2a56b311ae4a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ThrustDown"",
                    ""type"": ""Button"",
                    ""id"": ""9ad61787-ba99-4598-8824-58e31eb475e8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""45cc6cf9-5171-450d-b326-5c2318aabd2f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SeondaryFire"",
                    ""type"": ""Button"",
                    ""id"": ""c62361db-557b-4cc2-b0fb-0b425f1b8e57"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LockOn"",
                    ""type"": ""Button"",
                    ""id"": ""94e95428-9c00-41a2-af6e-a08ff506685e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""09c01a71-81e7-4957-9017-1969f4cb98ca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Step"",
                    ""type"": ""Button"",
                    ""id"": ""c1f97e84-3313-4f1d-a6cb-4fcd2b02b3ae"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""7b52d6b0-f03f-44cb-a338-e68a972a38dd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchTarget"",
                    ""type"": ""Value"",
                    ""id"": ""09f7d706-6abb-432a-9942-59c31f01eacb"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SwitchWeapon1"",
                    ""type"": ""Button"",
                    ""id"": ""ab5d5676-4713-407c-830b-f57602e34820"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchWeapon2"",
                    ""type"": ""Button"",
                    ""id"": ""d857c63c-6af7-469b-8ed6-3d4016d04cd6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchWeapon3"",
                    ""type"": ""Button"",
                    ""id"": ""a39bcd59-bd3b-425f-be83-413397ee49fa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""f519d9fc-ee30-44d9-8090-e4f60efd3fb9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SwitchWeaponSet"",
                    ""type"": ""Button"",
                    ""id"": ""c1b862db-7d2e-49c0-bb50-8f5a5b63cc9d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e814eda9-f176-410b-a6ab-26a3a316a32e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrustUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2dcea7c6-fdf5-4205-83e6-efc7ff35059b"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ThrustDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""09dc6395-49b0-4bfd-b32b-3b1a76365266"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5d1e7e7d-0e22-4baf-88e0-3f0b8de0fb48"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5cfc1e62-98f2-421f-8991-5a812fdb340b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d71f171d-01d1-48e4-8346-8c11468f3275"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""86783cb8-add3-4835-8038-85a269b3fea2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bd4d8c17-f57c-45b5-8768-581b457078d3"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c749f6a2-2a53-4ba9-a2e9-1f3fb251bceb"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5df7d2e8-28d0-4475-b114-77250ef6c8b4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SeondaryFire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1db36c8e-97a8-43d0-958f-b6f03c6f0bb5"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LockOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3bbf7412-24b2-401f-ae99-c272bcf7fcb5"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54211f7f-e37a-48fb-8a1c-cb11cc17fbd2"",
                    ""path"": ""<Mouse>/backButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Step"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5f923bc1-6171-45db-8816-71f2b79c16cb"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""df4ff8be-5153-4209-ab8c-d2d19efe849a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTarget"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""15eb3e63-5fa4-4c1d-bd0b-1c166ed0ceb3"",
                    ""path"": ""<Mouse>/scroll/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3f1e9baf-9238-474a-8ad4-abcfa8796f29"",
                    ""path"": ""<Mouse>/scroll/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""33e07646-e575-4097-ad27-1e0c6459ce02"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""235b85f1-9578-46ee-8437-eb28495237d1"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchTarget"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8607867b-b52c-447b-bfac-c859ff071c78"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchWeapon1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f733b9d-f7bb-4722-b1b7-4e71b380e921"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchWeapon2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9eb4d135-5905-4a31-9f11-ac170f920ecb"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchWeapon3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1119b27-54cd-4912-995a-92514e6f037c"",
                    ""path"": ""<Keyboard>/#(R)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cac1fe9-738c-4a52-bf68-9bab37a45702"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchWeaponSet"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_ThrustUp = m_Player.FindAction("ThrustUp", throwIfNotFound: true);
        m_Player_ThrustDown = m_Player.FindAction("ThrustDown", throwIfNotFound: true);
        m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
        m_Player_SeondaryFire = m_Player.FindAction("SeondaryFire", throwIfNotFound: true);
        m_Player_LockOn = m_Player.FindAction("LockOn", throwIfNotFound: true);
        m_Player_Aim = m_Player.FindAction("Aim", throwIfNotFound: true);
        m_Player_Step = m_Player.FindAction("Step", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
        m_Player_SwitchTarget = m_Player.FindAction("SwitchTarget", throwIfNotFound: true);
        m_Player_SwitchWeapon1 = m_Player.FindAction("SwitchWeapon1", throwIfNotFound: true);
        m_Player_SwitchWeapon2 = m_Player.FindAction("SwitchWeapon2", throwIfNotFound: true);
        m_Player_SwitchWeapon3 = m_Player.FindAction("SwitchWeapon3", throwIfNotFound: true);
        m_Player_Reload = m_Player.FindAction("Reload", throwIfNotFound: true);
        m_Player_SwitchWeaponSet = m_Player.FindAction("SwitchWeaponSet", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_ThrustUp;
    private readonly InputAction m_Player_ThrustDown;
    private readonly InputAction m_Player_Fire;
    private readonly InputAction m_Player_SeondaryFire;
    private readonly InputAction m_Player_LockOn;
    private readonly InputAction m_Player_Aim;
    private readonly InputAction m_Player_Step;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_SwitchTarget;
    private readonly InputAction m_Player_SwitchWeapon1;
    private readonly InputAction m_Player_SwitchWeapon2;
    private readonly InputAction m_Player_SwitchWeapon3;
    private readonly InputAction m_Player_Reload;
    private readonly InputAction m_Player_SwitchWeaponSet;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @ThrustUp => m_Wrapper.m_Player_ThrustUp;
        public InputAction @ThrustDown => m_Wrapper.m_Player_ThrustDown;
        public InputAction @Fire => m_Wrapper.m_Player_Fire;
        public InputAction @SeondaryFire => m_Wrapper.m_Player_SeondaryFire;
        public InputAction @LockOn => m_Wrapper.m_Player_LockOn;
        public InputAction @Aim => m_Wrapper.m_Player_Aim;
        public InputAction @Step => m_Wrapper.m_Player_Step;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputAction @SwitchTarget => m_Wrapper.m_Player_SwitchTarget;
        public InputAction @SwitchWeapon1 => m_Wrapper.m_Player_SwitchWeapon1;
        public InputAction @SwitchWeapon2 => m_Wrapper.m_Player_SwitchWeapon2;
        public InputAction @SwitchWeapon3 => m_Wrapper.m_Player_SwitchWeapon3;
        public InputAction @Reload => m_Wrapper.m_Player_Reload;
        public InputAction @SwitchWeaponSet => m_Wrapper.m_Player_SwitchWeaponSet;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @ThrustUp.started += instance.OnThrustUp;
            @ThrustUp.performed += instance.OnThrustUp;
            @ThrustUp.canceled += instance.OnThrustUp;
            @ThrustDown.started += instance.OnThrustDown;
            @ThrustDown.performed += instance.OnThrustDown;
            @ThrustDown.canceled += instance.OnThrustDown;
            @Fire.started += instance.OnFire;
            @Fire.performed += instance.OnFire;
            @Fire.canceled += instance.OnFire;
            @SeondaryFire.started += instance.OnSeondaryFire;
            @SeondaryFire.performed += instance.OnSeondaryFire;
            @SeondaryFire.canceled += instance.OnSeondaryFire;
            @LockOn.started += instance.OnLockOn;
            @LockOn.performed += instance.OnLockOn;
            @LockOn.canceled += instance.OnLockOn;
            @Aim.started += instance.OnAim;
            @Aim.performed += instance.OnAim;
            @Aim.canceled += instance.OnAim;
            @Step.started += instance.OnStep;
            @Step.performed += instance.OnStep;
            @Step.canceled += instance.OnStep;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @SwitchTarget.started += instance.OnSwitchTarget;
            @SwitchTarget.performed += instance.OnSwitchTarget;
            @SwitchTarget.canceled += instance.OnSwitchTarget;
            @SwitchWeapon1.started += instance.OnSwitchWeapon1;
            @SwitchWeapon1.performed += instance.OnSwitchWeapon1;
            @SwitchWeapon1.canceled += instance.OnSwitchWeapon1;
            @SwitchWeapon2.started += instance.OnSwitchWeapon2;
            @SwitchWeapon2.performed += instance.OnSwitchWeapon2;
            @SwitchWeapon2.canceled += instance.OnSwitchWeapon2;
            @SwitchWeapon3.started += instance.OnSwitchWeapon3;
            @SwitchWeapon3.performed += instance.OnSwitchWeapon3;
            @SwitchWeapon3.canceled += instance.OnSwitchWeapon3;
            @Reload.started += instance.OnReload;
            @Reload.performed += instance.OnReload;
            @Reload.canceled += instance.OnReload;
            @SwitchWeaponSet.started += instance.OnSwitchWeaponSet;
            @SwitchWeaponSet.performed += instance.OnSwitchWeaponSet;
            @SwitchWeaponSet.canceled += instance.OnSwitchWeaponSet;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @ThrustUp.started -= instance.OnThrustUp;
            @ThrustUp.performed -= instance.OnThrustUp;
            @ThrustUp.canceled -= instance.OnThrustUp;
            @ThrustDown.started -= instance.OnThrustDown;
            @ThrustDown.performed -= instance.OnThrustDown;
            @ThrustDown.canceled -= instance.OnThrustDown;
            @Fire.started -= instance.OnFire;
            @Fire.performed -= instance.OnFire;
            @Fire.canceled -= instance.OnFire;
            @SeondaryFire.started -= instance.OnSeondaryFire;
            @SeondaryFire.performed -= instance.OnSeondaryFire;
            @SeondaryFire.canceled -= instance.OnSeondaryFire;
            @LockOn.started -= instance.OnLockOn;
            @LockOn.performed -= instance.OnLockOn;
            @LockOn.canceled -= instance.OnLockOn;
            @Aim.started -= instance.OnAim;
            @Aim.performed -= instance.OnAim;
            @Aim.canceled -= instance.OnAim;
            @Step.started -= instance.OnStep;
            @Step.performed -= instance.OnStep;
            @Step.canceled -= instance.OnStep;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @SwitchTarget.started -= instance.OnSwitchTarget;
            @SwitchTarget.performed -= instance.OnSwitchTarget;
            @SwitchTarget.canceled -= instance.OnSwitchTarget;
            @SwitchWeapon1.started -= instance.OnSwitchWeapon1;
            @SwitchWeapon1.performed -= instance.OnSwitchWeapon1;
            @SwitchWeapon1.canceled -= instance.OnSwitchWeapon1;
            @SwitchWeapon2.started -= instance.OnSwitchWeapon2;
            @SwitchWeapon2.performed -= instance.OnSwitchWeapon2;
            @SwitchWeapon2.canceled -= instance.OnSwitchWeapon2;
            @SwitchWeapon3.started -= instance.OnSwitchWeapon3;
            @SwitchWeapon3.performed -= instance.OnSwitchWeapon3;
            @SwitchWeapon3.canceled -= instance.OnSwitchWeapon3;
            @Reload.started -= instance.OnReload;
            @Reload.performed -= instance.OnReload;
            @Reload.canceled -= instance.OnReload;
            @SwitchWeaponSet.started -= instance.OnSwitchWeaponSet;
            @SwitchWeaponSet.performed -= instance.OnSwitchWeaponSet;
            @SwitchWeaponSet.canceled -= instance.OnSwitchWeaponSet;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnThrustUp(InputAction.CallbackContext context);
        void OnThrustDown(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnSeondaryFire(InputAction.CallbackContext context);
        void OnLockOn(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnStep(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnSwitchTarget(InputAction.CallbackContext context);
        void OnSwitchWeapon1(InputAction.CallbackContext context);
        void OnSwitchWeapon2(InputAction.CallbackContext context);
        void OnSwitchWeapon3(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnSwitchWeaponSet(InputAction.CallbackContext context);
    }
}
