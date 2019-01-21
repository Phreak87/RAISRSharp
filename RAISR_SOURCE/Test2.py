import numpy as np
import cv2

w = np.array([5,1])
v = np.array( [[1,2],[3,4]])
idx1 = np.array ([1,0])
idx2 = np.array ([0,1])

print (w)
print (w[idx1])
print (w[idx2])
print ('')
print (v)
print (v[:,idx1])
print (v[:,idx2])