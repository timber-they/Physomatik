module Main (main) where

main = putStrLn "Hello, Physomatik!"

g = 9.81
densityAir = 1.2041

toRadian = (*) (pi/180)
toDegree = (*) (180/pi)

getxPart (a,b) = cos (toRadian b) * a
getyPart (a,b) = sin (toRadian b) * a

getresAngle x y
    |x > 0 = toDegree (atan (y/x))
    |x == 0 = signum y * 90
    |y == 0 = 180
    |y > 0 = 180 - toDegree (atan (abs y / abs x))
    |otherwise = 180 + toDegree (atan (abs y / abs x))

getresVector vs =
    let xsum = sum (map getxPart vs)
        ysum = sum (map getyPart vs)
        vector = (sqrt (xsum * xsum + ysum * ysum),getresAngle xsum ysum)
    in
        if snd vector < 0
        then (fst vector, snd vector + 360)
        else vector

getdeltaSpeed = (*)

getnewPos v (x,y) step = (x + getxPart v * step, y + getyPart v * step)

getFG = (*)

getFL cw a p v = cw * 0.5 * a * p * v * v

getFN a m g = cos (toRadian a) * getFG m g

getFH a m g = sin (toRadian a) * getFG m g

getFR = (*)

getFRa f angle m g = getFR f (getFN angle m g)

getnewSpeed v a dt = v + getdeltaSpeed a dt

getnewSpeedFm v f dt m = getnewSpeed v (f/m) dt

getnewSpeedvec v f dt m = getresVector [(getnewSpeedFm (getxPart v) (getxPart f) dt m, 0),(getnewSpeedFm (getyPart v) (getyPart f) dt m, 90)]

getnewPosSpeed fWurf m g cw a p step oldpos oldv =
    let newSpeed = getnewSpeedvec oldv (getresVector [fWurf,(getFG m g, 270),(getFL cw a p (fst oldv), snd oldv + 180)]) step m
    in
        (getxPart newSpeed * step + fst oldpos, getyPart newSpeed * step + snd oldpos, newSpeed)

getSpeedatShot fthrow anglethrow m g cw a p t t_throw step
    |t <= 0 = (0,0)
    |otherwise =
        let v = getSpeedatShot fthrow anglethrow m g cw a p (t-step) t_throw step
            fs = getresVector [(m*g, 270),(fthrow,anglethrow),(getFL cw a p (fst v), snd v + 180)]
        in  getnewSpeedvec v fs step m

getPosatShot fthrow anglethrow m g cw a p t tthrow step
    |t <= 0 = (0,0)
    |otherwise =
        let speed = getSpeedatShot fthrow anglethrow m g cw a p t tthrow step
            oldpos = getPosatShot fthrow anglethrow m g cw a p (t-step) tthrow step
        in (fst oldpos + getxPart speed * step, snd oldpos + getyPart speed * step)

notbigger01 a b
    |a > b = 0.0
    |otherwise = 1.0

getnewSpeedafterImpact v0 step cw a p m g t f angle =
    let vx = getxPart v0
        vy = getyPart v0
        fk = getF (fst (fst (getParts (fst v0) (snd v0 + 180) (angle + 90) angle))) t m
        fN = getFN angle m g + fst (fst (getParts fk (snd v0 + 180) (angle + 90) angle))
        fH = getFH angle m g
        fL = getFL cw a p (fst v0)
        fR = getFR f fN
        fResa = getresVector [(fH, angle + 180), (fL, snd v0 + 180), (fR, snd v0 + 180)]
        fRes = fst (getParts (fst fResa) (snd fResa) angle (angle + 90))
    in getresVector [(vx + getxPart fRes / m * step, 0), (vy + getyPart fRes / m * step, 90)]

getF v t m = (v/t) * m

getParts res resangle angle1 angle2 =
    let xParts = getxParts res resangle angle1 angle2
    in  ((fst (fst xParts) / cos (toRadian (snd (fst xParts))), snd (fst xParts)),(snd (fst xParts) / cos (toRadian (snd (snd xParts))), snd (snd xParts)))

getxParts res resangle angle1 angle2 = ((getonexPart res resangle angle2 angle1, angle1),(getonexPart res resangle angle1 angle2, angle2))

getonexPart res resangle otherangle ownangle =
    let a = toRadian otherangle
        b = toRadian resangle
        c = toRadian ownangle
    in (res * tan a * cos b - res * sin b) / (tan a - tan c)

getnewSpeedatHill f angle m g fs step v0 cw a p =
    let v = negatel v0 angle * fst v0
    in if v > 0 then v + ((fs - getFH angle m g - getFRa f angle m g - getFL cw a p v) / m) * step
                else v + ((fs - getFH angle m g + getFRa f angle m g + getFL cw a p v) / m) * step

getnewPosatHill f angle m g fs step v0 cw a p pos0 =
    let newspeed = getnewSpeedatHill f angle m g fs step v0 cw a p
    in  (fst pos0 + getxPart (newspeed*step, angle), snd pos0 + getyPart (newspeed * step, angle))

getnewPosSpeedatHill f angle m g fs step v0 cw a p pos0 =
    let newSpeed = (getnewSpeedatHill f angle m g fs step v0 cw a p, angle)
    in  if fst newSpeed < 0 then (getnewPosatHill f angle m g fs step v0 cw a p pos0, (negate (fst newSpeed), snd newSpeed + 180))
        else (getnewPosatHill f angle m g fs step v0 cw a p pos0, newSpeed)

negatel v0 angle
    |snd v0 > Main.mod (angle + 180) 360 - 5 && snd v0 < Main.mod (angle + 180) 360 + 5 = -1
    |otherwise = 1

mod a b
    |a > b = Main.mod (a-b) b
    |otherwise = a



















