import tensorflow as tf
import tensorflow.keras.backend as K
import os
import glob
from data import create_datagen
from model import get_model
import numpy as np
from metrics import psnr
from options import dir, batch_size

print(os.path.join(dir, 'X', '*.jpg'))
train = glob.glob(os.path.join(dir, 'X', '*.jpg'))
train = np.asarray(train)
print(len(train))

train_ds = create_datagen(train)
model = get_model()
initial_learning_rate = 1e-3
lr_schedule = tf.keras.optimizers.schedules.ExponentialDecay(
    initial_learning_rate,
    decay_steps=1000000,
    decay_rate=0.97,
    staircase=True)
#model = tf.keras.models.load_model('UnetFFAnomul_Syn7_YUV_50_ITS.h5', custom_objects={'MSEContent':MSEContent, 'PSNR':PSNR})
model.compile(optimizer = tf.keras.optimizers.Adam(learning_rate=lr_schedule), loss = tf.keras.losses.MSE, metrics=[psnr])
model.fit(train_ds.batch(batch_size = batch_size).shuffle(buffer_size = 100), epochs = 40)