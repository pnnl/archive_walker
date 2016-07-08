MapInfoPath = "D:/DISAT_PDAT/FOdet/"

SubNames = c("ALSN", "ALVY", "ASHE", 
             "BELL", "BGED", "BIGE",
             "CPJK", "CEFE", "CHJO", "CUST", 
             "DOJN", "DOOL", 
             "ECOL",
             "GARR", "GRIZ", "GCFI", "GCTW", "GRTW",
             "JDAY", "JDTW",
             "KEEL", "KNIT",
             "LOMO", 
             "MALN", "MARN", "MCNY", "MONE",
             "NBON",
             "OSTD", 
             "PAUL", "PERL",
             "RKCR", 
             "SHUL", "SLAT", "SUML", 
             "TROU", "TAFT",
             "WAUT")

SubLats = c(46.108, 43.999915, 46.47635, 
            47.7524, 45.603515, 45.603515,
            42.078668, 46.597297, 47.98604, 48.908412, 
            46.505187, 45.727127,
            47.495516,
            46.515229, 44.483586, 47.96603, 47.96603, 47.96603,
            45.678, 45.678,
            45.54977, 45.878492,
            46.55464,
            42.007399, 44.799722, 45.92522, 47.899103,
            45.64851,
            45.362624,
            46.7545, 45.32977,
            45.783545,
            47.12122, 45.70367, 43.020918, 
            45.559, 47.4575, 
            46.51593)

SubLons = c(-123.034, -123.0147, -119.33616, 
            -117.3722, -121.107704, -121.107704,
            -121.391637, -117.808678, -119.65092, -122.627025, 
            -117.847305, -120.825206,
            -121.873037,
            -112.912338, -121.01963, -119.02395, -119.02395, -119.02395,
            -120.736, -120.736,
            -122.89582, -120.887139,
            -118.53937,
            -121.318074, -122.698611, -119.31358, -121.890126,
            -121.96008,
            -122.400831,
            -122.8743, -122.77764,
            -120.530935,
            -120.49689, -120.15286, -120.957223,
            -122.4029, -115.5896, 
            -119.83621)

# The following 3 lines get the map from the internet. Because an internet connection
# may not be available, the map is saved to an RData file for retrieval. To change
# the map, connect to the internet and run the code below, which should normally be
# commented out. Alternatively, run these lines on a separate computer that is connected
# to the internet and move the resulting RData file to the computer running DISAT.
# 
# library(ggmap)
# WECCmap <- get_map(location = c(lon = -118, lat = 45.5),maptype="toner", zoom = 6)
# save(WECCmap=WECCmap, file = paste(MapInfoPath,"WECCmap.RData",sep=""))
# 
# This line simply loads the map that is saved in the above lines of code. Saving the map
# and loading it here makes it so the computer running DISAT does not need an internet
# connection to retrieve WECCmap every time this script is run.
#
load(paste(MapInfoPath,"WECCmap.RData",sep=""))


save(SubNames=SubNames, SubLats=SubLats, SubLons=SubLons, WECCmap=WECCmap,
     file = paste(MapInfoPath,"SCmapInfo.RData",sep=""))