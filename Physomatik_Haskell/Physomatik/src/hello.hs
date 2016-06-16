module Main (main) where

import Data.Maybe
import Data.Data
import System.IO
import Data.Char
import Control.Monad

main = do simulateAndWriteFromFile "data.txt" "end.txt"

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
            fs = getresVector [(m*g, 270), (fthrow * notbigger01 t t_throw , anglethrow),(getFL cw a p (fst v), snd v + 180)]
        in  getnewSpeedvec v fs step m

getnewSpeedatShot fthrow anglethrow m g cw a p t t_throw step v
    |t <= 0 = (0,0)
    |otherwise =
        let fs = getresVector [(m*g, 270), (fthrow * notbigger01 t t_throw , anglethrow),(getFL cw a p (fst v), snd v + 180)]
        in getnewSpeedvec v fs step m

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

getnewSpeedatHill :: Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> Double -> Double -> Double -> Double
getnewSpeedatHill f angle m g fs step v0 cw a p =
    let v = negatel v0 angle * fst v0
    in if v > 0 then v + ((fs - getFH angle m g - getFRa f angle m g - getFL cw a p v) / m) * step
                else v + ((fs - getFH angle m g + getFRa f angle m g + getFL cw a p v) / m) * step

--                              f           angle   m           g           fs      step        v0              cw          a       p           t_throw   t
getnewSpeedatHillwitht_throw :: Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> Double -> Double -> Double -> Double -> Double -> Double
getnewSpeedatHillwitht_throw f angle m g fs step v0 cw a p t_throw t
    |t <= t_throw = getnewSpeedatHill f angle m g fs step v0 cw a p
    |otherwise = getnewSpeedatHill f angle m g 0 step v0 cw a p

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

simulatePosatShot arena f_throw angle_throw t_throw f_floor cw p a v_old pos_old step m g t max
    |length arena == 0 || length (head arena) == 0  || fst pos_old < 0.11 || snd pos_old < 0.11 = []
    |t <= max && round (fst pos_old) < length (head arena) && round (snd pos_old) < length arena && fst pos_old > 0 && snd pos_old > 0=
        let pos_pixl = (round (fst pos_old), round (snd pos_old))
            pos_speed = getnewPosSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t pos_old
        in fst pos_speed : simulatePosatShot arena f_throw angle_throw t_throw f_floor cw p a (snd pos_speed) (fst pos_speed) step m g (t+step) max
    |otherwise = []

getfromPos1 ::[a] -> Int -> a
getfromPos1 a pos = head (take 1 (drop pos a))
getfromPos2 :: [[a]] -> Int -> Int -> a
getfromPos2 a posy posx = head $ take 1 $ drop posy $ head $ take 1 $ drop posx a

getnewSpeedwithMap :: [[Maybe Double]] -> Double -> Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> (Int,Int) -> Double -> Double -> Double -> Double -> (Double, Double)
getnewSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t
    |isNothing (getfromPos2 arena (fst pos_pixl) (snd pos_pixl)) = getnewSpeedatShot f_throw angle_throw m g cw a p t t_throw step v_old
    |otherwise = (getnewSpeedatHillwitht_throw f_floor (maybenormal (getfromPos2 arena (fst pos_pixl) (snd pos_pixl))) m g f_throw step v_old cw a p t_throw t , maybenormal (getfromPos2 arena (fst pos_pixl) (snd pos_pixl)))

getnewPoswithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t pos_old =
    let v = getnewSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t
    in getnewPos v pos_old step

getnewPosSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t pos_old =
    let v = getnewSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t
    in (getnewPos v pos_old step, v)

maybenormal (Just a) = a
maybenormal Nothing = 0

fileto2dList path divider0 divider1= do
    content <- readFile path
    return (stringto2dList content divider0 divider1)

fileto1dList :: String -> Char -> IO [String]
fileto1dList path divider = do
    content <- readFile path
    return (stringto1dList content divider)

getfirstPartofstring string divider
    |head string == divider = []
    |null (tail string) = [head string]
    |otherwise = head string : getfirstPartofstring (tail string) divider

stringto1dList string divider =
    let firstpart = getfirstPartofstring string divider
        lastpart = drop (length firstpart + 1) string
    in  if null lastpart then [firstpart]
            else firstpart : stringto1dList lastpart divider

onedto2dList [] divider = []
onedto2dList onedlist divider = stringto1dList (head onedlist) divider : onedto2dList (tail onedlist) divider

stringto2dList string divider0 = onedto2dList (stringto1dList string divider0)

stringtoMaybe ['J','u','s','t',' ',x] = Just (fromIntegral (digitToInt x) * 36.0)
stringtoMaybe "Nothing" = Nothing

onedstringtoMaybe = map stringtoMaybe
twodstringtoMaybe = map onedstringtoMaybe

simulatefromFile :: Double -> Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> (Double,Double) -> Double -> Double -> Double -> Double -> Double -> String -> Char -> Char -> IO [(Double,Double)]
simulatefromFile f_throw angle_throw t_throw f_floor cw p a v_old pos_old step m g t  max filePath divider0 divider1 = do
    x <- fileto2dMaybe filePath divider0 divider1
    return (simulatePosatShot x f_throw angle_throw t_throw f_floor cw p a v_old pos_old step m g t  max)

getdata :: String -> Char -> IO (Double, Double, Double, Double, Double, Double, Double, (Double, Double), (Double, Double), Double, Double, Double, Double, Double, String, Char, Char)
getdata filePath divider = do
    l <- fileto1dList filePath divider
    let x = (read (head l) :: Double, read (getfromPos1 l 1) :: Double, read (getfromPos1 l 2) :: Double, read (getfromPos1 l 3) :: Double, read (getfromPos1 l 4) ::Double,
            read (getfromPos1 l 5) :: Double, read (getfromPos1 l 6) :: Double, stringToPair (getfromPos1 l 7) :: (Double,Double), stringToPair (getfromPos1 l 8) :: (Double,Double),
            read (getfromPos1 l 9) :: Double, read (getfromPos1 l 10) :: Double, read (getfromPos1 l 11) :: Double, read (getfromPos1 l 12) :: Double, read (getfromPos1 l 13) :: Double,
            getfromPos1 l 14 :: String, head (getfromPos1 l 15) :: Char, head (getfromPos1 l 16) :: Char)
    return x

stringToPair ('(':x) = (read (stringUntily x ','), read (stringUntily (stringFromy x ',') ')'))
stringToPair _ = (0.0,0.0)

stringUntily (x:xs) y
    | x == y = []
    |otherwise = x : stringUntily xs y

stringFromy (x:xs) y
    |x == y = xs
    |otherwise = stringFromy xs y

simulatefromFiles filePath_data = do
    content <- getdata "data.txt" ':'
    simulatefromFile
        (g1OQ (showDataTuple content 0)) (g1OQ (showDataTuple content 1)) (g1OQ (showDataTuple content 2)) (g1OQ (showDataTuple content 3))
        (g1OQ (showDataTuple content 4)) (g1OQ (showDataTuple content 5)) (g1OQ (showDataTuple content 6)) (g2OQ (showDataTuple content 7))
        (g2OQ (showDataTuple content 8)) (g1OQ (showDataTuple content 9)) (g1OQ (showDataTuple content 10)) (g1OQ (showDataTuple content 11))
        (g1OQ (showDataTuple content 12)) (g1OQ (showDataTuple content 13)) (g3OQ (showDataTuple content 14)) (g4OQ (showDataTuple content 15))
        (g4OQ (showDataTuple content 16))

fileto2dMaybe filePath divider0 divider1 = do
    x <- fileto2dList filePath divider0 divider1
    return (twodstringtoMaybe x)

showDataTuple :: (Double, Double, Double, Double, Double, Double, Double, (Double,Double), (Double,Double), Double, Double, Double, Double, Double, String, Char, Char) -> Int -> (Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 0 = (Just x1, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 1 = (Just x2, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 2 = (Just x3, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 3 = (Just x4, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 4 = (Just x5, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 5 = (Just x6, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 6 = (Just x7, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 7 = (Nothing, Just x8, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 8 = (Nothing, Just x9, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 9 = (Just x10, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 10 = (Just x11, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 11 = (Just x12, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 12 = (Just x13, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 13 = (Just x14, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 14 = (Nothing, Nothing, Just x15, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 15 = (Nothing, Nothing, Nothing, Just x16)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) 16 = (Nothing, Nothing, Nothing, Just x17)

showAllDataTuple x 17 = []
showAllDataTuple x y = showDataTuple x y : showAllDataTuple x (y+1)

getxofDataFile filePath x = do
    y <- getdata"data.txt" ':'
    let z = showDataTuple y x
    return z

g1OQ (Just a, _, _, _) = a
g2OQ (_, Just a, _, _) = a
g3OQ (_, _, Just a, _) = a
g4OQ (_, _, _, Just a) = a

listToString [] = ""
listToString (x : xs) = (show x) ++ (listToString xs)

simulateAndWriteFromFile datafile resultfile = do
	x <- simulatefromFiles datafile
	writeFile resultfile (show x)

test = simulatefromFile 0 1 2 3 4 5 6 (7,7) (8,8) 0.001 10 11 12 13 "hi.txt" '.' ','
test2 = do
    x <- fileto2dMaybe "hi.txt" '.' ','
    return (length  x)

test3 = do
    (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17) <- getdata "data.txt" ':'
    print x2
    return x2
