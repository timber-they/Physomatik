--nc -> not controlled
--hc -> hopefully correct
module Main (main) where

import Data.Maybe
import Data.Data
import System.IO
import Data.Char
import Control.Monad

main = do simulateAndWriteFromFile "data.txt" "end.txt"

g::Double
g = 9.81
densityAir::Double
densityAir = 1.2041

toRadian::Double -> Double
toRadian = (*) (pi/180)
toDegree::Double -> Double
toDegree = (*) (180/pi)

--returns the x-/y-Part of an vector
getxPart:: (Double,Double) -> Double
getxPart (a,b) = cos (toRadian b) * a
getyPart:: (Double,Double) -> Double
getyPart (a,b) = sin (toRadian b) * a

--Part of getresVector, returns the resulting angle of two values (x/y)
getresAngle::Double -> Double -> Double
getresAngle x y
    |x > 0 = toDegree (atan (y/x))
    |x == 0 = signum y * 90
    |y == 0 = 180
    |y > 0 = 180 - toDegree (atan (abs y / abs x))
    |otherwise = 180 + toDegree (atan (abs y / abs x))

--returns the resutling Vector of two Vectors
getresVector::[(Double,Double)] -> (Double,Double)
getresVector vs =
    let xsum = sum (map getxPart vs)
        ysum = sum (map getyPart vs)
        vector = (sqrt (xsum * xsum + ysum * ysum),getresAngle xsum ysum)
    in
        if snd vector < 0
        then (fst vector, snd vector + 360)
        else vector

--returns the Speed-difference at given acceleration and time
getdeltaSpeed::Double->Double->Double
getdeltaSpeed = (*)

--returns the new position at given Speed, Old Position and Step
getnewPos::(Double,Double) -> (Double,Double) -> Double -> (Double,Double)
getnewPos v (x,y) step = (x + getxPart v * step, y + getyPart v * step)

--returns the Power of Earth
getFG::Double->Double->Double
getFG = (*)

--returns the Power of Air
getFL::Double->Double->Double->Double->Double
getFL cw a p v = cw * 0.5 * a * p * v * v

--returns the Power of the object pressing on the floor
getFN::Double->Double->Double->Double
getFN a m g = cos (toRadian a) * getFG m g

--returns the Power of the object pulling it downwards - todo: Power direction wrong!!
getFH::Double->Double->Double->Double
getFH a m g = -abs(sin (toRadian a) * getFG m g)

--returns the BAD Power
getFR::Double->Double->Double
getFR = (*)

--returns the BAD power at not-straight-floors
getFRa::Double->Double->Double->Double->Double
getFRa f angle m g = getFR f (getFN angle m g)

--returns the new Speed at given Old Speed, acceleration and time-span
getnewSpeed::Double->Double->Double->Double
getnewSpeed v a dt = (+) v (getdeltaSpeed a dt)

--returns the new Speed at given old Speed, Power, time-span and mass
getnewSpeedFm::Double->Double->Double->Double->Double
getnewSpeedFm v f dt m = getnewSpeed v (f/m) dt

--returns the new Speed at given old Speed-vector, Power-vector, time-span and mass
getnewSpeedvec::(Double,Double)->(Double,Double)->Double->Double->(Double,Double)
getnewSpeedvec v f dt m = getresVector [(getnewSpeedFm (getxPart v) (getxPart f) dt m, 0),(getnewSpeedFm (getyPart v) (getyPart f) dt m, 90)]

--returns the new Position and Speed at a throw
getnewPosSpeed::(Double,Double)->Double->Double->Double->Double->Double->Double->(Double,Double)->(Double,Double)->((Double,Double),(Double,Double))
getnewPosSpeed fWurf m g cw a p step oldpos oldv =
    let newSpeed = getnewSpeedvec oldv (getresVector [fWurf,(getFG m g, 270),(getFL cw a p (fst oldv), snd oldv + 180)]) step m
    in
        ((getxPart newSpeed * step + fst oldpos, getyPart newSpeed * step + snd oldpos), newSpeed)

--returns the Speed at a Shot
getSpeedatShot::Double->Double->Double->Double->Double->Double->Double->Double->Double->Double->(Double,Double)
getSpeedatShot fthrow anglethrow m g cw a p t t_throw step
    |t <= 0 = (0,0)
    |otherwise =
        let v = getSpeedatShot fthrow anglethrow m g cw a p (t-step) t_throw step
            fs = getresVector [(getFG m g, 270), (fthrow * notbigger01 t t_throw , anglethrow),(getFL cw a p (fst v), snd v + 180)]
        in  getnewSpeedvec v fs step m

--returns the new Speed at a Shot
getnewSpeedatShot::Double->Double->Double->Double->Double->Double->Double->Double->Double->Double->(Double,Double)->(Double,Double)
getnewSpeedatShot fthrow anglethrow m g cw a p t t_throw step v
    |t <= 0 = (0,0)
    |otherwise =
        let fs = getresVector [(getFG m g, 270), (fthrow * notbigger01 t t_throw , anglethrow),(getFL cw a p (fst v), snd v + 180)]
        in getnewSpeedvec v fs step m

--returns the Position at a Shot
getPosatShot::Double->Double->Double->Double->Double->Double->Double->Double->Double->Double->(Double,Double)
getPosatShot fthrow anglethrow m g cw a p t tthrow step
    |t <= 0 = (0,0)
    |otherwise =
        let speed = getSpeedatShot fthrow anglethrow m g cw a p t tthrow step
            oldpos = getPosatShot fthrow anglethrow m g cw a p (t-step) tthrow step
        in (fst oldpos + getxPart speed * step, snd oldpos + getyPart speed * step)

--returns wether a is not bigger than b (1/0)
notbigger01::Double->Double->Double
notbigger01 a b
    |a > b = 0.0
    |otherwise = 1.0

--returns the new Speed after an Impact -nc
getnewSpeedafterImpact::(Double,Double)->Double->Double->Double->Double->Double->Double->Double->Double->Double->(Double,Double)
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

--F = a * m
getF::Double->Double->Double->Double
getF v t = (*) (v/t)

--returns two parts of a Vector -hc
getParts::Double->Double->Double->Double->((Double,Double),(Double,Double))
getParts res resangle angle1 angle2 =
    let xParts = getxParts res resangle angle1 angle2
        a = fst (snd xParts) / (cos (toRadian (snd (snd xParts))))
        b = fst (fst xParts) / (cos (toRadian (snd (fst xParts))))
    in  ((b, angle1), (a, angle2))

--returns two x-Parts of a Vector -hc
getxParts::Double->Double->Double->Double->((Double,Double),(Double,Double))
getxParts res resangle angle1 angle2 = ((getonexPart res resangle angle2 angle1, angle1),(getonexPart res resangle angle1 angle2, angle2))

--returns one x-Part of a Vector (with Intersection Point and wxMaxima) -hc
getonexPart::Double->Double->Double->Double->Double
getonexPart res resangle otherangle ownangle =
    let a = toRadian otherangle
        b = toRadian resangle
        c = toRadian ownangle
    in (res * tan a * cos b - res * sin b) / (tan a - tan c)

--returns the new Speed at a Hill
getnewSpeedatHill :: Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> Double -> Double -> Double -> Double -> Bool -> (Double,Double)
getnewSpeedatHill f angle m g fs step v0 cw a p bouncefac up =
    let parts = getParts (fst v0) (snd v0) angle (angle+270)
        vt  |fst (snd parts) > 0 && up || fst (snd parts) < 0 && (not up) = getresVector [fst parts, ((fst (snd parts) * bouncefac), (snd (snd parts)))]
            |otherwise = getresVector [fst parts, snd parts]
        fn = getresVector [(getFH angle m g, angle), (getFRa f angle m g, snd vt + 180), (getFL cw a p (fst vt), snd vt + 180)]
    in  getnewSpeedvec vt fn step m

--returns the new Speed at a Hill with throwing-Power/Time
--                              f           angle   m           g           fs      step        v0              cw          a       p           t_throw   t
getnewSpeedatHillwitht_throw :: Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> Double -> Double -> Double -> Double -> Double -> Double -> Bool -> (Double, Double)
getnewSpeedatHillwitht_throw f angle m g fs step v0 cw a p t_throw t bouncefac up
    |t <= t_throw = getnewSpeedatHill f angle m g fs step v0 cw a p bouncefac up
    |otherwise = getnewSpeedatHill f angle m g 0 step v0 cw a p bouncefac up

--returns the new Pos at a Hill
getnewPosatHill::Double->Double->Double->Double->Double->Double->(Double,Double)->Double->Double->Double->(Double,Double)->Double->(Double,Double)
getnewPosatHill f angle m g fs step v0 cw a p pos0 bouncefac=
    let newspeed = getnewSpeedatHill f angle m g fs step v0 cw a p bouncefac True
    in  (fst pos0 + getxPart (fst newspeed*step, snd newspeed), snd pos0 + getyPart (fst newspeed * step, snd newspeed))

--returns the new Position and Speed at a Hill
getnewPosSpeedatHill::Double->Double->Double->Double->Double->Double->(Double,Double)->Double->Double->Double->(Double,Double)->Double->((Double,Double),(Double,Double))
getnewPosSpeedatHill f angle m g fs step v0 cw a p pos0 bouncefac =
    let newSpeed = getnewSpeedatHill f angle m g fs step v0 cw a p bouncefac True
    in  if fst newSpeed < 0 then (getnewPosatHill f angle m g fs step v0 cw a p pos0 bouncefac, (negate (fst newSpeed), snd newSpeed + 180))
        else (getnewPosatHill f angle m g fs step v0 cw a p pos0 bouncefac, newSpeed)

--negates it maybe (in addiction to the angle)
negatel::(Double,Double)->Double->Double
negatel v0 angle
    |snd v0 > Main.mod (angle + 180) 360 - 5 && snd v0 < Main.mod (angle + 180) 360 + 5 = -1
    |otherwise = 1

-- %
mod::Double->Double->Double
mod a b
    |a > b = Main.mod (a-b) b
    |otherwise = a

--simulates a whole Position List at a Shot
simulatePosatShot::[[Maybe Double]]->Double->Double->Double->Double->Double->Double->Double->(Double,Double)->(Double,Double)->Double->Double->Double->Double->Double->Double->[(Double,Double)]
simulatePosatShot arena f_throw angle_throw t_throw f_floor cw p a v_old pos_old step m g t max bouncefac
    |length arena == 0 || length (head arena) == 0  || fst pos_old < 0.11 || snd pos_old < 0.11 = [(1,1)]
    |t <= max && round (fst pos_old) < length (head arena) && round (snd pos_old) < length arena && fst pos_old > 0 && snd pos_old > 0=
        let pos_pixl = (floor (fst pos_old), floor (snd pos_old))
            pos_speed = getnewPosSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t pos_old bouncefac
        in fst pos_speed : simulatePosatShot arena f_throw angle_throw t_throw f_floor cw p a (snd pos_speed) (fst pos_speed) step m g (t+step) max bouncefac
    |otherwise = [(2,2)]

--returns the item of a 1d List a at the Position pos
getfromPos1 ::[a] -> Int -> a
getfromPos1 a pos = head (take 1 (drop pos a))
--returns the item of a 2d List a at the Position posx, posy
getfromPos2 :: [[a]] -> Int -> Int -> a
getfromPos2 a posy posx = head $ take 1 $ drop posy $ head $ take 1 $ drop posx a

--returns the new Speed at hill/air in addiction to the position in the map
getnewSpeedwithMap :: [[Maybe Double]] -> Double -> Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> (Int,Int) -> Double -> Double -> Double -> Double -> Double -> Bool -> (Double, Double)
getnewSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t bouncefac up
    |isNothing (getfromPos2 arena (fst pos_pixl) (snd pos_pixl)) = getnewSpeedatShot f_throw angle_throw m g cw a p t t_throw step v_old
    |otherwise = getnewSpeedatHillwitht_throw f_floor (maybenormal (getfromPos2 arena (fst pos_pixl) (snd pos_pixl))) m g f_throw step v_old cw a p t_throw t bouncefac up

--returns the new Position at hill/air in addiction to the position in the map
getnewPoswithMap::[[Maybe Double]]->Double->Double->Double->Double->Double->Double->Double->(Double,Double)->(Int,Int)->Double->Double->Double->Double->(Double,Double)->Double->(Double,Double)
getnewPoswithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t pos_old bouncefac =
    let v = getnewSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t bouncefac (isUp (snd pos_old))
    in getnewPos v pos_old step

--returns the new Position and the new Speed at hill/air in addiction to the position in the map
getnewPosSpeedwithMap::[[Maybe Double]]->Double->Double->Double->Double->Double->Double->Double->(Double,Double)->(Int,Int)->Double->Double->Double->Double->(Double,Double)->Double->((Double,Double),(Double,Double))
getnewPosSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t pos_old bouncefac=
    let v = getnewSpeedwithMap arena f_throw angle_throw t_throw f_floor cw p a v_old pos_pixl step m g t bouncefac (isUp (snd pos_old))
    in (getnewPos v pos_old step, v)

--converts something of type Maybe a to a or 0
maybenormal::Num a => Maybe a -> a
maybenormal (Just a) = a
maybenormal Nothing = 0

--Reads a File and converts it to a 2d List
fileto2dList::String->Char->Char->IO[[String]]
fileto2dList path divider0 divider1= do
    content <- readFile path
    return (stringto2dList content divider0 divider1)

--Reads a File and converts it to a 1d List
fileto1dList :: String -> Char -> IO [String]
fileto1dList path divider = do
    content <- readFile path
    return (stringto1dList content divider)

--returns the Part of a String until the divider
getfirstPartofstring::String->Char->String
getfirstPartofstring string divider
    |head string == divider = []
    |null (tail string) = [head string]
    |otherwise = head string : getfirstPartofstring (tail string) divider

--converts a String to a 1d List with help of the divider
stringto1dList::String->Char->[String]
stringto1dList string divider =
    let firstpart = getfirstPartofstring string divider
        lastpart = drop (length firstpart + 1) string
    in  if null lastpart then [firstpart]
            else firstpart : stringto1dList lastpart divider

--converts a 1d to a 2d List with help of the divider
onedto2dList::[String]->Char->[[String]]
onedto2dList [] divider = []
onedto2dList onedlist divider = stringto1dList (head onedlist) divider : onedto2dList (tail onedlist) divider

--converts a Srting to a 2d List with help of the dividers
stringto2dList::String->Char->Char->[[String]]
stringto2dList string divider0 = onedto2dList (stringto1dList string divider0)

--Converts a String (like "Just 5") to a Maybe (like Just 5)
stringtoMaybe::String->Maybe Double
stringtoMaybe ['J','u','s','t',' ',x] = Just (fromIntegral (digitToInt x))
stringtoMaybe "Nothing" = Nothing
stringtoMaybe ['J','u','s','t',' ', x, y] = Just (fromIntegral (digitToInt x * 10 + digitToInt y))
stringtoMaybe ['J','u','s','t',' ', x, y, z] = Just (fromIntegral (digitToInt x * 100 + digitToInt y * 10 + digitToInt z))

--converts a List of Strings to a List of Maybes
onedstringtoMaybe::[String]->[Maybe Double]
onedstringtoMaybe = map stringtoMaybe
--converts a 2d List of Strings to a 2d List of Maybes
twodstringtoMaybe::[[String]]->[[Maybe Double]]
twodstringtoMaybe = map onedstringtoMaybe

--simulates all the Position from a arena got with a File
simulatefromFile :: Double -> Double -> Double -> Double -> Double -> Double -> Double -> (Double,Double) -> (Double,Double) -> Double -> Double -> Double -> Double -> Double -> String -> Char -> Char -> Double -> IO [(Double,Double)]
simulatefromFile f_throw angle_throw t_throw f_floor cw p a v_old pos_old step m g t  max filePath divider0 divider1 bouncefac = do
    x <- fileto2dMaybe filePath divider0 divider1
    return (simulatePosatShot x f_throw angle_throw t_throw f_floor cw p a v_old pos_old step m g t max bouncefac)

--gets the data out of the data File
getdata :: String -> Char -> IO (Double, Double, Double, Double, Double, Double, Double, (Double, Double), (Double, Double), Double, Double, Double, Double, Double, String, Char, Char, Double)
getdata filePath divider = do
    l <- fileto1dList filePath divider
    let x = (read (head l) :: Double, read (getfromPos1 l 1) :: Double, read (getfromPos1 l 2) :: Double, read (getfromPos1 l 3) :: Double, read (getfromPos1 l 4) ::Double,
            read (getfromPos1 l 5) :: Double, read (getfromPos1 l 6) :: Double, stringToPair (getfromPos1 l 7) :: (Double,Double), stringToPair (getfromPos1 l 8) :: (Double,Double),
            read (getfromPos1 l 9) :: Double, read (getfromPos1 l 10) :: Double, read (getfromPos1 l 11) :: Double, read (getfromPos1 l 12) :: Double, read (getfromPos1 l 13) :: Double,
            getfromPos1 l 14 :: String, head (getfromPos1 l 15) :: Char, head (getfromPos1 l 16) :: Char, read (getfromPos1 l 17) :: Double)
    return x

--converts a String (like "(5.23,2.856)") to a Pair (like (5.23,2.856))
stringToPair::String->(Double,Double)
stringToPair ('(':x) = (read (stringUntily x ','), read (stringUntily (stringFromy x ',') ')'))
stringToPair _ = (0.0,0.0)

--returns the String until the y
stringUntily::String->Char->String
stringUntily (x:xs) y
    | x == y = []
    |otherwise = x : stringUntily xs y

--returns the String from y
stringFromy::String->Char->String
stringFromy (x:xs) y
    |x == y = xs
    |otherwise = stringFromy xs y

--simulates the positions from a given data-file
simulatefromFiles::String->IO [(Double,Double)]
simulatefromFiles filePath_data = do
    content <- getdata "data.txt" ':'
    simulatefromFile
        (g1OQ (showDataTuple content 0)) (g1OQ (showDataTuple content 1)) (g1OQ (showDataTuple content 2)) (g1OQ (showDataTuple content 3))
        (g1OQ (showDataTuple content 4)) (g1OQ (showDataTuple content 5)) (g1OQ (showDataTuple content 6)) (g2OQ (showDataTuple content 7))
        (g2OQ (showDataTuple content 8)) (g1OQ (showDataTuple content 9)) (g1OQ (showDataTuple content 10)) (g1OQ (showDataTuple content 11))
        (g1OQ (showDataTuple content 12)) (g1OQ (showDataTuple content 13)) (g3OQ (showDataTuple content 14)) (g4OQ (showDataTuple content 15))
        (g4OQ (showDataTuple content 16)) (g1OQ (showDataTuple content 17))

--reads a 2d Maybe List out of a File (map)
fileto2dMaybe::String->Char->Char->IO [[Maybe Double]]
fileto2dMaybe filePath divider0 divider1 = do
    x <- fileto2dList filePath divider0 divider1
    return (turnListAround(twodstringtoMaybe x))

--returns the List turned around
turnListAround::[a]->[a]
turnListAround [] = []
turnListAround list = last list : turnListAround (init list)

--ugly Data Tuple Stuff -> gives a data Point from given data Tuple and Position
showDataTuple :: (Double, Double, Double, Double, Double, Double, Double, (Double,Double), (Double,Double), Double, Double, Double, Double, Double, String, Char, Char, Double) -> Int -> (Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 0 = (Just x1, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 1 = (Just x2, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 2 = (Just x3, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 3 = (Just x4, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 4 = (Just x5, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 5 = (Just x6, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 6 = (Just x7, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 7 = (Nothing, Just x8, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 8 = (Nothing, Just x9, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 9 = (Just x10, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 10 = (Just x11, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 11 = (Just x12, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 12 = (Just x13, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 13 = (Just x14, Nothing, Nothing, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 14 = (Nothing, Nothing, Just x15, Nothing)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 15 = (Nothing, Nothing, Nothing, Just x16)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 16 = (Nothing, Nothing, Nothing, Just x17)
showDataTuple (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) 17 = (Just x18, Nothing, Nothing, Nothing)

--shows you all the data Points
showAllDataTuple::(Double, Double, Double, Double, Double, Double, Double, (Double,Double), (Double,Double), Double, Double, Double, Double, Double, String, Char, Char, Double)->Int->[(Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)]
showAllDataTuple x 18 = []
showAllDataTuple x y = showDataTuple x y : showAllDataTuple x (y+1)

--gives a Data Point from a data File
getxofDataFile::String->Int->IO (Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)
getxofDataFile filePath x = do
    y <- getdata"data.txt" ':'
    let z = showDataTuple y x
    return z

--gives the first item of the Data Point Tuple
g1OQ::(Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)->Double
g1OQ (Just a, _, _, _) = a
--gives the second item of the Data Point Tuple
g2OQ::(Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)->(Double,Double)
g2OQ (_, Just a, _, _) = a
--gives the third item of the Data Point Tuple
g3OQ::(Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)->String
g3OQ (_, _, Just a, _) = a
--gives the fourth item of the Data Point Tuple
g4OQ::(Maybe Double, Maybe (Double, Double), Maybe String, Maybe Char)->Char
g4OQ (_, _, _, Just a) = a

--converts a List to a String
listToString::Show a=>[a]->String
listToString [] = ""
listToString (x : xs) = (show x) ++ (listToString xs)

--Simulates all the Positions from a data File and saves it into a File
simulateAndWriteFromFile::String->String->IO ()
simulateAndWriteFromFile datafile resultfile = do
    x <- simulatefromFiles datafile
    writeFile resultfile (show x)

isUp::Double -> Bool
isUp a = (<) 0.5 $ afterDot a

afterDot::Double -> Double
afterDot a = (-) a $ fromInteger $ floor a

test = simulatefromFile 0 1 2 3 4 5 6 (7,7) (8,8) 0.001 10 11 12 13 "hi.txt" '.' ','
test2 = do
    x <- fileto2dMaybe "hi.txt" '.' ','
    return (length  x)

test3 = do
    (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18) <- getdata "data.txt" ':'
    print x2
    return x1