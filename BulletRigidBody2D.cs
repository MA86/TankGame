using Godot;
using System;

public class BulletRigidBody2D : RigidBody2D
{
    // Declare variables here
    Timer bulletTimer;

    // Called when the node enters the scene tree
    public override void _Ready()
    {
        // This timer is used to time bullet in the air
        bulletTimer = new Timer();
        bulletTimer.SetWaitTime(2);
        bulletTimer.Connect("timeout", this, "OnBulletTimerTimeout");
        bulletTimer.Start();
        this.AddChild(bulletTimer);

        this.Connect("body_entered", this, "OnBulletBodyEntered");
        this.ContactMonitor = true;
        this.ContactsReported = 10;

        // Hide the bullet for now
        this.Hide();
    }

    // Fires bullet when called
    public void FireBullet()
    {
        float magnitude = 2000;
        Vector2 direction = this.GlobalTransform.y;
        this.ApplyImpulse(this.Position, direction * magnitude);
    }

    // Destroys bullet when called
    void OnBulletTimerTimeout()
    {
        this.QueueFree();
    }

    // On bullet impact, explode
    void OnBulletBodyEntered(Node body)
    {
        if (body is TankKinematicBody2D)
        {
            Sprite explosion = this.GetNode<Sprite>("Explosion");
            AnimationPlayer animation = this.GetNode<AnimationPlayer>("Explosion/AnimationPlayer");

            // First remove explosion sprite from current parent in order to assign a new parent
            this.RemoveChild(explosion);

            // Add explosion sprite as a child of main scene, so explosion don't move 
            this.GetParent().AddChild(explosion);
            explosion.Position = this.Position;
            animation.Play("explosion");
            this.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();

            // Destroy both tank and bullet
            body.QueueFree();
            // this.QueueFree();
            this.Disconnect("body_entered", this, "OnBulletBodyEntered");
        }
    }
}
