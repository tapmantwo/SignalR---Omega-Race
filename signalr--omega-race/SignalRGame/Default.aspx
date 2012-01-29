<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SignalRGame._Default" %>
<html>
    <head>
        <title>Oh Mega Race!</title>
        <script type="text/javascript" src="Scripts/jquery-1.6.4.min.js"></script>
        <script type="text/javascript" src="Scripts/json2.min.js"></script>
        <script type="text/javascript" src="Scripts/jquery.signalR.min.js"></script>

        <script type="text/javascript">
            $(document).ready(function () {

                // Connection used to deal with players joining - either through this instance or other instances
                // (therefore a two way connection)
                var playerConnection = $.connection('playerconnection');
                playerConnection.start();

                playerConnection.received(function (data) {
                    var playerName = data.Name;
                    if (isThisPlayer(playerName)) {
                        startProcessingKeyPresses();
                        hideJoinBox();
                    }
                    displayWelcomeMessage(playerName, data.Colour);
                });

                // Send only connection used to send key presses to the game engine
                var keyPressConnection = $.connection('keypressconnection');

                // Connection to the game engine this is effectively a receive only
                // connection, used by the engine to tell us to perform some ui action
                var gameConnection = $.connection('gameconnection');
                gameConnection.start();

                gameConnection.received(function (data) {
                    ships = data.Ships;
                    enemies = data.Enemies;
                    explodedShips = data.Blasts;
                    arena = data.Arena;
                    refreshScreen();
                });

                function startProcessingKeyPresses() {
                    inGame = true;
                    keyPressConnection.start();
                }

                function hideJoinBox() {
                    $('#login').hide();
                }

                function displayWelcomeMessage(playerName, shipColour) {
                    var message = 'Welcome to the game, ' + playerName + '.';
                    var notificationArea = $('#notifications');
                    notificationArea.css('color', shipColour);
                    notificationArea.html(message).fadeIn(3000, function () {
                        notificationArea.html(message).fadeOut(3000);
                    });
                }

                function isThisPlayer(playerName) {
                    return (playerName == currentPlayerName);
                }

                function join() {
                    var playerName = $('#playername').val();
                    currentPlayerName = playerName;
                    tellServerPlayerJoined(playerName);
                }

                function tellServerPlayerJoined(playerName) {
                    playerConnection.send(playerName);
                }

                var ships = [];
                var enemies = [];
                var currentPlayerName = '';
                var inGame = false;
                var context;
                var shipsize = 30;
                var arena;
                var explodedShips = [];
                var bplus = 6; //used to set the varied starburst effect of the ship and enemy blast

                //this draws the two squares of the enemyship that rotate
                function drawEnemy(enemy) {
                    context.save();
                    context.translate(enemy.X, enemy.Y);
                    context.lineWidth = 1;
                    context.strokeStyle = enemy.Colour;
                    context.strokeRect(-5, -5, 10, 10);
                    context.beginPath();
                    context.arc(0, 0, 17, 0, Math.PI * 2, true);
                    context.closePath();
                    context.stroke();
                    context.lineWidth = 1;
                    context.translate(0, 0);
                    for (var b = 0; b < 2; b++) {
                        context.translate(0, 0);
                        context.rotate(150);
                        context.strokeRect(-13, -13, 26, 26);
                    }
                    context.restore();
                }

                function drawEnemies() {
                    for (var i = 0; i < enemies.length; i++) {
                        var enemy = enemies[i];
                        drawEnemy(enemy);
                    }
                }

                function drawExplodedShips() {
                    for (var i = 0; i < explodedShips.length; i++) {
                        var explodedShip = explodedShips[i];
                        for (var k = 0; k < 11; k++) {
                            bplus = bplus * -1;
                            context.strokeStyle = "white";
                            context.beginPath();
                            context.moveTo(explodedShip.X + 10 * Math.cos(k * 20), explodedShip.Y + 10 * Math.sin(k * 20));
                            context.lineTo(explodedShip.X + (25 + bplus) * Math.cos(k * 20), explodedShip.Y + (25 + bplus) * Math.sin(k * 20));
                            context.closePath();
                            context.stroke();
                        }
                        if (explodedShip.Angle < 2) {
                            explodedShip.Angle = 0;
                        } else {
                            explodedShip.Angle--;
                        }
                        if (explodedShip.Angle == 0) {
                            explodedShips.splice(i, 1);
                        }
                    }
                }


                function drawArena() {
                    context.save();
                    context.beginPath();
                    context.strokeStyle = "yellow";
                    // Draw the central reservation area
                    var centralReservation = arena.CentralReservation;
                    context.strokeRect(centralReservation.X, centralReservation.Y, centralReservation.Width, centralReservation.Height);
                    // Draw the frame
                    var boundary = arena.Boundary;
                    context.strokeRect(boundary.X, boundary.Y, boundary.Width, boundary.Height);
                    context.closePath();
                    context.stroke();
                    context.restore();
                }

                function drawShip(ship) {
                    context.save();
                    context.strokeStyle = ship.Colour;
                    context.translate(ship.X, ship.Y);
                    context.rotate(ship.Angle * Math.PI / 180);
                    context.beginPath();
                    context.moveTo(shipsize * -.3, shipsize * .2);
                    context.lineTo(shipsize * -.4, shipsize * -.1);
                    context.lineTo(shipsize * .1, shipsize * .4);
                    context.lineTo(0, shipsize * .5);
                    context.lineTo(shipsize * -.1, shipsize * .4);
                    context.lineTo(shipsize * .4, shipsize * -.1);
                    context.lineTo(shipsize * .3, shipsize * .2);
                    context.moveTo(shipsize * -.4, shipsize * -.1);
                    context.lineTo(shipsize * .3, shipsize * -.5);
                    context.lineTo(0, shipsize * .3);
                    context.lineTo(shipsize * -.3, shipsize * -.5);
                    context.lineTo(shipsize * .4, shipsize * -.1);
                    context.stroke();
                    context.restore();

                    drawLasers(ship);
                }

                function drawShips() {
                    for (var i = 0; i < ships.length; i++)
                        drawShip(ships[i]);
                }

                function keyDown(e) {
                    var msg = 'd:' + currentPlayerName + ':' + e.keyCode;
                    keyPressConnection.send(msg);
                }

                function keyUp(e) {
                    var msg = 'u:' + currentPlayerName + ':' + e.keyCode;
                    keyPressConnection.send(msg);
                }

                //this explains where and how to draw your laser missile.  For every laser you have it will add the missile speed to  the last lasers last x & y coordinate, and it will position the x & yat an angle that the ship was in when you fired
                function drawLasers(ship) {
                    for (var i = 0; i < ship.Missiles.length; i++) {
                        var missile = ship.Missiles[i];
                        drawLaser(missile, ship.Colour);
                    }
                }

                //this draws the diamond pattern of the laser fire
                function drawLaser(missile, colour) {
                    context.save();
                    context.translate(missile.X, missile.Y);
                    context.rotate(missile.Angle * Math.PI / 180);
                    context.strokeStyle = colour;
                    context.beginPath();
                    context.moveTo(0, shipsize * .2);
                    context.lineTo(shipsize * -.1, 0);
                    context.lineTo(0, shipsize * -.2);
                    context.lineTo(shipsize * .1, 0);
                    context.lineTo(0, shipsize * .2);
                    context.stroke();
                    context.restore();
                }

                // This is the core animation of the game.  It clears everything then redraws all items again based on their new position calculated in each function
                function refreshScreen() {
                    game_area.width = arena.Width;
                    game_area.height = arena.Height;
                    context = game_area.getContext('2d');
                    context.clearRect(0, 0, arena.Width, arena.Height);
                    drawArena();
                    drawEnemies();
                    drawShips();
                    drawExplodedShips();
                }

                // Wire up the key presses etc
                $(document).keydown(function (e) {
                    if (inGame)
                        keyDown(e);
                });

                $(document).keyup(function (e) {
                    if (inGame)
                        keyUp(e);
                });

                $('#join').click(function () {
                    join();
                });

            });
        </script>
        <style type="text/css">
         #game_area{
             background-color:#000000;
             }
        </style>
    </head>
    <body>
        <canvas id="game_area"></canvas>
        <div><label id="notifications"/></div>
        <div id= "login"><input type="text" id="playername"/><a id='join' href="#">join</a></div>
    </body>
</html>


