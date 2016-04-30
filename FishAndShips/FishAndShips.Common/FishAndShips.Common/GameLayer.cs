using System;
using System.Collections.Generic;
using CocosSharp;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Linq;

namespace FishAndShips.Common
{
    public class GameLayer : CCLayerColor
    {
        CCSprite fishSprite;
        CCSprite netSprite;
        CCSprite shipSprite;
        CCLabel scoreLabel;
        long gameStartTimeMillisecounds;
        long previousFrameMillisecounds;
        long lastZoomMillisecounds;
        float fishCenterYCoordinate = Constants.Height / 2;
        float targetCenterYCoordinate = Constants.Height / 2;
        CCDrawNode drawNode;
        bool gameStarted = false;
        int fishAmplitude = 200;

        int score;

        public GameLayer() : base(CCColor4B.Black)
        {
            Init();            
        }

        public void Init()
        {
            // "paddle" refers to the paddle.png image
            fishSprite = new CCSprite("fish");
            fishSprite.PositionX = 70;
            fishSprite.PositionY = fishCenterYCoordinate;
            fishSprite.ContentSize = new CCSize(120, 60);
            AddChild(fishSprite);

            netSprite = new CCSprite("net");
            netSprite.PositionX = Constants.Width;
            netSprite.PositionY = Constants.Height - 235;
            netSprite.ContentSize = new CCSize(300, 470);
            AddChild(netSprite);

            shipSprite = new CCSprite("ship");
            shipSprite.PositionX = Constants.Width;
            shipSprite.PositionY = 235;
            shipSprite.ContentSize = new CCSize(256, 76);
            AddChild(shipSprite);

            scoreLabel = new CCLabel("Score: 0", "Arial", 50, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = Constants.Width - 300;
            scoreLabel.PositionY = Constants.Height;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            drawNode = new CCDrawNode();
            AddChild(drawNode);
        }

        void RunGameLogic(float frameTimeInSeconds)
        {
            // New Code:
            // Check if the two CCSprites overlap...
            bool collision = netSprite.BoundingBoxTransformedToParent.IntersectsRect(
                fishSprite.BoundingBoxTransformedToParent);

            bool collision2 = shipSprite.BoundingBoxTransformedToParent.IntersectsRect(
                fishSprite.BoundingBoxTransformedToParent);

            if (collision || collision2)
            {
                scoreLabel.Text = "Game over! Score: " + score + "\nTap to restart!";
                scoreLabel.SystemFontSize = 100;
                scoreLabel.PositionX = Constants.Width / 2;
                scoreLabel.PositionY = Constants.Height / 2;
                scoreLabel.AnchorPoint = CCPoint.AnchorMiddle;
                gameStarted = false;
            }
            else
            {               
                long currentTimeMillisecounds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                netSprite.PositionX = Constants.Width - (((currentTimeMillisecounds - gameStartTimeMillisecounds) / 2) % Constants.Width);
                shipSprite.PositionX = Constants.Width - (((currentTimeMillisecounds - gameStartTimeMillisecounds) / 3) % Constants.Width);

                if(fishCenterYCoordinate > targetCenterYCoordinate)
                {
                    fishCenterYCoordinate -= (int)((currentTimeMillisecounds - previousFrameMillisecounds) / 3d);
                }
                else if (fishCenterYCoordinate < targetCenterYCoordinate)
                {
                    fishCenterYCoordinate += (int)((currentTimeMillisecounds - previousFrameMillisecounds) / 3d);
                }
                
                drawNode.Clear();
                drawNode.DrawLine(new CCPoint(0, fishCenterYCoordinate), new CCPoint(Constants.Width, fishCenterYCoordinate), new CCColor4B(100, 100, 100));
                drawNode.DrawLine(new CCPoint(0, fishCenterYCoordinate - fishAmplitude), new CCPoint(Constants.Width, fishCenterYCoordinate - fishAmplitude), new CCColor4B(139, 0, 0));
                drawNode.DrawLine(new CCPoint(0, fishCenterYCoordinate + fishAmplitude), new CCPoint(Constants.Width, fishCenterYCoordinate + fishAmplitude), new CCColor4B(139, 0, 0));

                fishSprite.PositionY = (fishCenterYCoordinate + (int)(Math.Sin(currentTimeMillisecounds / 400d) * fishAmplitude));
                if (fishSprite.PositionY < 0)
                    fishSprite.PositionY = 0;
                if (fishSprite.PositionY > Constants.Height)
                    fishSprite.PositionY = Constants.Height;

                score = (int)((currentTimeMillisecounds - gameStartTimeMillisecounds) / 1000);
                scoreLabel.Text = "Score: " + score;
                previousFrameMillisecounds = currentTimeMillisecounds;
            }
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            CCRect bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;
            touchListener.OnTouchesMoved = HandleTouchesMoved;
            
            AddEventListener(touchListener, this);
        }

        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                if(!gameStarted)
                {
                    RemoveAllChildren();
                    Init();
                    gameStartTimeMillisecounds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    previousFrameMillisecounds = gameStartTimeMillisecounds;
                    Schedule(RunGameLogic);
                    gameStarted = true;
                }
            }
        }

        void HandleTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count == 1)
            {
                if((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - lastZoomMillisecounds > 500)
                    targetCenterYCoordinate = (int)touches[0].Location.Y;
            }
            if (touches.Count > 1)
            {
                var touch = touches.OrderByDescending(t => t.Location.Y).First();

                if (touch.PreviousLocation.Y > touch.Location.Y)
                {
                    if (fishAmplitude > 100)
                    {
                        int deltaY = (int)(touch.PreviousLocation.Y - touch.Location.Y);
                        fishAmplitude -= deltaY;
                        if (fishAmplitude < 100)
                            fishAmplitude = 100;                   
                    }
                }
                else if (touch.PreviousLocation.Y < touch.Location.Y)
                {
                    if (fishAmplitude < 300)
                    {
                        int deltaY = (int)(touch.Location.Y - touch.PreviousLocation.Y);
                        fishAmplitude += deltaY;
                        if (fishAmplitude > 300)
                            fishAmplitude = 300;
                    }
                }
                lastZoomMillisecounds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                targetCenterYCoordinate = fishCenterYCoordinate;
            }            
        }
    }
}

