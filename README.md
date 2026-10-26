# Lab_9

**About the Game** <br>
This is a carnival duck shooting gallery. The player controls a rifle at the bottom of the screen and shoots ducks as they ride across three wooden shelves. The ducks come out from behind the curtains on one side, bob up and down as they move, and disappear behind the curtains on the other side. The shelves alternate direction. Each duck type is a different size and speed and is worth a different number of points, and the score updates every time a duck it hit.
**Controls:** A / D (or the left and right arrow keys) move the rifle, and holding the left mouse button (or Enter) fires the bullet.

**Design Patterns** <br>
**Object Pool (bullets)**<br>
Files: BulletPool.cs, Bullet.cs, ShooterController.cs <br>
Instead of instantiating a bullet every time the player shoots and destroying it when it's done, this reuses the same set of bullets.
* In Awake(), BulletPool instantiates 20 bullets, turns them off and puts them in a Queue<Bullet>.
* When the player fires, ShooterController calls bulletPool.Get(), which takes a bullet out of the queue. Bullet.Launch() then moves it to the fire point at the tip of the rifle, turns it on and sends it straight up.
* When a bullet hits a duck or goes past the top of the screen, it calls pool.Return(this). That turns the bullet off and puts it back in the queue so it can be fired again.
* If all 20 bullets are in the air at once, Get() makes one more and that bullet stays in the pool from then on. Return() ignores a bullet that is already turned off, so one bullet can't end up in the queue twice.

During play, the number of bullets under the BulletPool object in the Hierarchy stays the same. They just turn on and offs.

**Builder (ducks)**
Files: ITargetBuilder.cs, TargetBuilding.cs, TargetDirector.cs, TargetConfig.cs, TargetSpawner.cs
This is set up the same way as the spaceship example from class:
* ITargetBuilding (the interface) lists the steps for putting a duck together: SetSprite, SetColor, SetSize, SetSpeed, SetPointValue, SetPosition, SetDirection, and Build. Each step returns the builder, so the calls can be chained.
* TargetDirector (the Builder instructions) has Construct(config, position, direction), which calls the builder steps in order for one duck type.
* TargetConfig (the configurations) are ScriptableObject assets that hold the values for each premade duck type: Yellow Duck (size = 0.7, speed = 4, points = 50); White Duck (size = 1, speed = 2.5, points = 20); Brown Duck (size = 1.2, speed = 1.5, points = 10).
* TargetBuilder (the concrete builder) stores each value as the director sets it. When Build() is called, it instantiates the single Target prefab, scales it, moves it up so the bottom of the duck sits on its shelf no matter what size it is, and passes the sprite, speed, points, and direction to the target.

TargetSpawner handles the timing. Every spawn interval, it picks a random shelf and a random duck config and asks the director to build the duck just past the edge of the screen, behind the curtain. Every duck comes from the same prefab, so adding a new duck type only means making a new TargetConfig asset. No code has to change.

**Observer (score)**
Files: Target.cs, ScoreManager.cs
* **Subject:** Target has public static event Action<int> OnTargetHit. When a bullet hits a duck, Bullet calls target.Hit(), which raises the event with that duck's point value and then removes the duck. A flag makes sure a duck only counts once if two bullets hit it in the same frame.
* **Observer:** ScoreManager subscribes to OnTargetHit in OnEnable() and unsubscribes in OnDisable(). When the event fires, HandleTargetHit(points) adds the points to the score and updates the score text.

The ducks don't have a reference to ScoreManager and don't know anything about it. They only raise the event. Something else, like a sound effect or a high score tracker, could subscribe to OnTargetHit later without any changes to Target.

**Other Scripts**
* ShooterController - moves the rifle and keeps it on screen, and fires bullets from the pool
* ScreenBounds - gets the edges of teh screen from the camera so nothing has hardcoded screen positions

**Assets**
The art is from Kenny's Shooting Gallery pack (CC0): https://kenney.nl/assets/shooting-gallery 
