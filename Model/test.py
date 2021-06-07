import tensorflow as tf
from metrics import psnr
from data import parse_image
import numpy as np
import time
import cv2
from options import dir, model_name
import os
import glob


model = tf.keras.models.load_model(model_name, custom_objects={'PSNR':psnr})

test_imgs = (glob.glob(os.path.join(dir, '*.png')) + glob.glob(os.path.join(dir, '*.jpg')) + glob.glob(os.path.join(dir, '*.jpeg')))
if not os.path.exists('out'):
    os.makedirs('out')

for img_name in test_imgs:
    print(img_name)
    img = parse_image(img_name, test = True)
    crop_x = 0
    crop_y = 0
    img = img.numpy()
    #print(img.shape)
    if(img.shape[0] % 2 != 0):
        img = np.pad(img, ((1, 0),(0, 0), (0, 0)), 'reflect')
        crop_x = 1
    if(img.shape[1] % 2 != 0):
        img = np.pad(img, ((0, 0),(1, 0), (0, 0)), 'reflect')
    crop_y = 1
    img_x = ((img.shape[0] // 64 + 1) * 64 - img.shape[0]) // 2
    img_y = ((img.shape[1] // 64 + 1) * 64 - img.shape[1]) // 2
    img = np.pad(img, ((img_x, img_x),(img_y, img_y), (0, 0)), 'reflect')
    start = time.time()
    pred = (model.predict(np.expand_dims(img, axis = 0)))
    pred[0] = tf.image.yuv_to_rgb(pred[0])
    end = time.time()
    print('time elapsed:',end - start)
    cv2.imwrite(os.path.join('out', os.path.basename(img_name)), ((pred[0][img_x + crop_x:-img_x, img_y + crop_y:-img_y, 3::-1])) * 255)