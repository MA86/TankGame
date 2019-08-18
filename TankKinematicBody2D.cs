using Godot;
using System;

public class TankKinematicBody2D : KinematicBody2D
{
    // Declare member variables here
    int tankSpeed = 60;
    int turretSpeed = 1;
    PackedScene bulletGenerator;
    int moveForward;
    int moveBackward;
    int rotateLeft;
    int rotateRight;
    int rotateTurretLeft;
    string fireCannon;
    int rotateTurretRight;
    [Export]
    bool moveWithWASD;

    // Called when the node enters the scene tree
    public override void _Ready()
    {
        // Default movement is with arrow keys
        SetMoveWithArrows();
        
        // If desired, movement can be changed to WASD for the other player
        if (moveWithWASD)
            SetMoveWithWASD();

        // Load the bullet scene
        bulletGenerator = (PackedScene)GD.Load("res://BulletRigidBody2D.tscn");

        // Hide the animation for now
        this.GetNode<AnimatedSprite>("Sprite/Sprite2/Sprite/AnimatedSprite").Hide();
    }

    // Called every frame
    public override void _PhysicsProcess(float delta)
    {
        RotateTankIfNeeded();
        MoveTankIfNeeded();
        FireBulletIfNeeded();
        RotateTurretIfNeeded();
    }

    // Set control keys to arrows
    public void SetMoveWithArrows()
    {
        moveForward = (int)KeyList.Up;
        moveBackward = (int)KeyList.Down;
        rotateLeft = (int)KeyList.Left;
        rotateRight = (int)KeyList.Right;

        rotateTurretRight = (int)KeyList.Shift;
        rotateTurretLeft = (int)KeyList.Slash;

        fireCannon = "ui_accept";
    }

    // Set control keys to WASD
    public void SetMoveWithWASD()
    {
        moveForward = (int)KeyList.W;
        moveBackward = (int)KeyList.S;
        rotateLeft = (int)KeyList.A;
        rotateRight = (int)KeyList.D;

        rotateTurretRight = (int)KeyList.Q;
        rotateTurretLeft = (int)KeyList.E;

        fireCannon = "ui_t";
    }

    // Spawn a bullet if the space key is pressed
    void FireBulletIfNeeded()
    {

        if (Input.IsActionJustPressed(fireCannon))
        {
            // Position the bullet and fire
            BulletRigidBody2D bullet = (BulletRigidBody2D)bulletGenerator.Instance();
            bullet.Position = this.GetNode<AnimatedSprite>("Sprite/Sprite2/Sprite/AnimatedSprite").GlobalPosition;
            bullet.Rotation = this.GetNode<Sprite>("Sprite/Sprite2/Sprite").GlobalRotation + Mathf.Deg2Rad(0);
            this.GetParent().AddChild(bullet);
            bullet.FireBullet();
            bullet.Show();
            this.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();

            // Play the fire animation
            this.GetNode<AnimatedSprite>("Sprite/Sprite2/Sprite/AnimatedSprite").Show();
            this.GetNode<AnimatedSprite>("Sprite/Sprite2/Sprite/AnimatedSprite").SetFrame(0);
            this.GetNode<AnimatedSprite>("Sprite/Sprite2/Sprite/AnimatedSprite").Play();
        }
    }

    // Moves the tank if the correct keys are pressed.
    void MoveTankIfNeeded()
    {
        if (Input.IsKeyPressed(moveForward) || Input.IsKeyPressed(moveBackward))
        {
            this.MoveAndSlide(GetVelocityVector());
        }
    }

    // Prepare a movement velocity vector based on user input
    Vector2 GetVelocityVector()
    {
        Vector2 velocity = new Vector2();

        if (Input.IsKeyPressed(moveForward))
        {
            velocity = new Vector2(0, 1).Rotated(this.Rotation);
        }

        if (Input.IsKeyPressed(moveBackward))
        {
            velocity = (new Vector2(0, 1).Rotated(this.Rotation)) * -1;
        }

        velocity = velocity.Normalized() * tankSpeed;

        return velocity;
    }

    // User input transformed to tank rotation
    void RotateTankIfNeeded()
    {
        if (Input.IsKeyPressed(rotateRight))
        {
            this.Rotation += Mathf.Deg2Rad(1);
        }

        if (Input.IsKeyPressed(rotateLeft))
        {
            this.Rotation -= Mathf.Deg2Rad(1);
        }
    }

    // function to rotate turret
    void RotateTurretIfNeeded()
    {
        Sprite turret = this.GetNode<Sprite>("Sprite/Sprite2");

        if (Input.IsKeyPressed(rotateTurretRight))
        {
            turret.Rotate(Mathf.Deg2Rad(turretSpeed));
        }

        if (Input.IsKeyPressed(rotateTurretLeft))
        {
            turret.Rotate(Mathf.Deg2Rad(turretSpeed * -1));
        }
    }
}
