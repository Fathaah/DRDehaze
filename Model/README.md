## UNET model

Implementation of a convolution based UNET model.

### Dependencies 

* Python 3.0
* OpenCV 3.4
* Tensorflow-GPU 2.0
* Numpy 1.16

### Architecture

<p align="center">
  <img src=UNET_DEHAZE.png>
</p>


### Train

Training can be carried out by executing `main.py` with options `--dir`, `--batch_size` for the dataset directory and batch size respectively. A sample of this command is shown below:

```shell
python main.py --dir=dir --batch_size=2  
```

### Test

Testing can be carried out by executing `test.py` with options `--dir` and `--model` for the test set directory and the saved model. Trained model is included in this repository.

```shell
python test.py --dir=val --model=UnetFFAnomul_DR.h5
```
