import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--batch_size",type=int, help = "batch size")
parser.add_argument("--dir",type=str, help = "Data Directory")
args = parser.parse_args()

dir = args.dir 
batch_size = args.batch_size