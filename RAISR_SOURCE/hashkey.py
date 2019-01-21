﻿import numpy as np
import cv2
from math import atan2, floor, pi

def hashkey(block, Qangle, W):
    # Calculate gradient
    gy, gx = np.gradient(block)

    cv2.imshow ("test",gy)

    # Transform 2D matrix into 1D array
    gx = gx.ravel()
    gy = gy.ravel()

    # SVD calculation
    G = np.vstack((gx,gy)).T
    GTWG1 = G.T.dot(W)
    GTWG = GTWG1.dot(G)
    w, v = np.linalg.eig(GTWG);

    # Make sure V and D contain only real numbers
    nonzerow = np.count_nonzero(np.isreal(w))
    nonzerov = np.count_nonzero(np.isreal(v))
    if nonzerow != 0:
        w = np.real(w)
    if nonzerov != 0:
        v = np.real(v)

    # Sort w and v according to the descending order of w
    # w = np.array([[1,0,2],[5,4,6],[9,7,8]])
    idx = w.argsort()
    idx = idx[::-1]

    w = w[idx]
    v = v[:,idx]

    # Calculate theta
    tv1 = v[1,0]
    theta = atan2(tv1, v[0,0])
    if theta < 0:
        theta = theta + pi

    # Calculate lamda
    lamda = w[0]

    # Calculate u
    sqrtlamda1 = np.sqrt(w[0])
    sqrtlamda2 = np.sqrt(w[1])
    if sqrtlamda1 + sqrtlamda2 == 0:
        u = 0
    else:
        u = (sqrtlamda1 - sqrtlamda2)/(sqrtlamda1 + sqrtlamda2)

    # Quantize
    angle = floor(theta/pi*Qangle)
    if lamda < 0.0001:
        strength = 0
    elif lamda > 0.001:
        strength = 2
    else:
        strength = 1

    if u < 0.25:
        coherence = 0
    elif u > 0.5:
        coherence = 2
    else:
        coherence = 1

    # Bound the output to the desired ranges
    if angle > 23:
        angle = 23
    elif angle < 0:
        angle = 0

    return angle, strength, coherence
