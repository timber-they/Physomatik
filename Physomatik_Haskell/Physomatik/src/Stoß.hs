module Main (main) where

data Ralf = Ralf {position :: (Double,Double), mass :: Double, speed :: (Double,Double), size :: Double, cw :: Double, bouncefac :: Double}deriving (Show)

test = 
	let ralf = Ralf {
		position = (0,1), 
		mass = 2, 
		speed = (3,4), 
		size = 5, 
		cw = 6, 
		bouncefac = 7 }
	in (position ralf, mass ralf, speed ralf, size ralf, cw ralf, bouncefac ralf)

main = return 0