##' This function reads in a PDAT PMU file 
##' @export
##' @param filepath contains 
##'
##' @return No object returned
##'
##' @author Brett Amidan
##'
##' @examples
##' readDST_MP(input="D:/Data/Data.csv",rdata.path="D:/Test")

readPMU_PDAT <- function(filepath) {
#***************************************************************
# Reference:
#           1)  IEEE C37.118 standard  [Martin 1998]
#           2)	A data format specification from BPA (file name: 02_PDA data file format.docx).
#			      3)	A matlab code from BPA (provided by Tony Faris with BPA.)
  # the function is to read the "*.pdat" data files
  # by Meng Jia	mjia3@binghamton.edu
  # Binghamton, NY,	09/15/2014
  #
  #
  ## Input variables:
  #		filepath: the path to the "*.pdat" file 
  #
  #
  ## Output variables:
  #		PMU: a 2-dimension matrix stored all the data in PMUs
  #
  #
  ###
  
  require(compiler)  # Not sure this line and the following one should go here. They came from testpdatReadv06, which was the basis for readPMU_PDAT()
  enableJIT(3);
  
  dec2char <- function(d)   # a function to convert decimal to character
  {
    d <- d[!d==32]
    d <- rawToChar(as.raw(d))
    return(d)
  }
  dec2char <- cmpfun(dec2char);
  ###
  int2bin <- function(g)    # a function to convert integer to binary
  {
    m <- as.integer(intToBits(g))
    return(m[8:1])
  }
  int2bin <- cmpfun(int2bin);
  ###
  bin2dec <- function(x)    # a function to convert binary to decimal
  {
    d <- sum(2^(which(rev(unlist(strsplit(as.character(x), "")) == 1))-1))
    return(d)
  }
  bin2dec <- cmpfun(bin2dec);
  ###
  uint16 <- function(f)     # a function to convert datatypes to uint16
  {
    f1 <- as.integer(intToBits(f[1]))
    f2 <- as.integer(intToBits(f[2]))
    n <- c(f1[8:1],f2[8:1])
    n <- sum(2^(which(rev(unlist(strsplit(as.character(n), "")) == 1))-1))
    return(n)
  }
  uint16 <- cmpfun(uint16);
  ###
  int16 <- function(f)      # a function to convert datatypes to int16
  {
    n <- c(int2bin(f[1]),int2bin(f[2]))
    m <- bin2dec(n)
    if (n[1] == 1) 
    {
      n[n==0] <-2
      n[n==1] <-0
      n[n==2] <-1
      m <- -(bin2dec(n)+1)
    }
    return(m)
  }
  int16 <- cmpfun(int16);
  ###
  implode <- function(..., sep='') # a function to paste all the values in an array to a string
  {
    d <- paste(..., sep=sep, collapse=sep)
    return(d)
  }
  implode <- cmpfun(implode);
  ###
  hex2dec <- function(h)    #a function to convert hexadecimal to decimal
  {
    h <- implode(c(int2bin(h[1]),int2bin(h[2]),int2bin(h[3]),int2bin(h[4])))
    h <- bin2dec(as.numeric(h))
    return(h)
  }
  hex2dec <- cmpfun(hex2dec);
  ###
  int8 <- function(f)       # a function to convert datatypes to int8
  {
    if(f > 127) f <- 127 else
      if(f < -128) f <- -128
    return(f)
  }
  int8 <- cmpfun(int8);
  ###
  uint16int8 <- function(f)     # a function to convert datatypes to uint16
  {
    n <- c(int2bin(int8(f[1])),int2bin(int8(f[2])))
    return(bin2dec(n))
  }
  uint16int8 <- cmpfun(uint16int8);
  ###
  uint32int8 <- function(f)     # a function to convert datatypes to uint32
  {
    n <- c(int2bin(int8(f[1])),int2bin(int8(f[2])),int2bin(int8(f[3])),int2bin(int8(f[4])))
    return(bin2dec(n))
  }
  uint32int8 <- cmpfun(uint32int8);
  ###
  uint32 <- function(f)     # a function to convert datatypes to uint32
  {
    n <- c(int2bin(f[1]),int2bin(f[2]),int2bin(f[3]),int2bin(f[4]))
    return(bin2dec(n))
  }
  uint32 <- cmpfun(uint32);
  ###
  cart2pol <- function(r,i) # a function to transform Cartesian to polar coordinates
  {
    z <- complex(real=r,imaginary=i)
    m <- Mod(z)
    a <- Arg(z)
    a <- a*360/(2*pi)
    x <- c(m,a)
    return(x)
  }
  cart2pol <- cmpfun(cart2pol);
  ###
  readConfigFrame <- function(cfgFileData) # a function to read the configuration of the data
  {
    config.basic <- matrix(,10,1,dimnames=list(c("idcode","SOC","fracSec","timeBase","numPMUs","dataRate",
                                                 "dataFrameSize","numph","numan","numdg"),c()))	#define a matrix to store all the basic information of configuration
    config.sync <- uint16(cfgFileData[1:2])
    config.framesize <- uint16(cfgFileData[3:4])
    config.basic[1,1] <- uint16(cfgFileData[5:6])						#idcode
    config.basic[2,1] <- uint32(cfgFileData[7:10])						#SOC
    config.basic[3,1] <- uint32(cfgFileData[11:14])						#fracSec
    config.basic[4,1] <- uint32(cfgFileData[15:18])						#timeBase
    config.basic[5,1] <- uint16(cfgFileData[19:20])						#numPMUs
    numPMUs <- config.basic[5,1]
    
    count <- 21
    dataFrameCount <- 15
    
    for(ind in 1:numPMUs) 
    {
      if(ind==1) 	#define a matrix to store all the basic information of every PMU
      {
        PMU.name <- matrix(,1,numPMUs)
        PMU.basic <- matrix(,13,numPMUs,dimnames = list(c("idcode", "fmt","frqFmt","anFmt","phFmt",
                                                          "phCoord","numPhsrs","numAnlgs","numDigs","pmuOffset","frqOffset","nomFrq","cfgCount"),c()))
      }
      PMU.name[1,ind] <- dec2char(cfgFileData[count:(count+15)])		#name
      PMU.basic[1,ind] <- uint16(cfgFileData[(count+16):(count+17)])	#idcode
      PMU.basic[2,ind] <- uint16int8(cfgFileData[(count+18):(count+19)])#fmt
      
      PMU.basic[7,ind] <- uint16int8(cfgFileData[(count+20):(count+21)])#numPhsrs
      PMU.basic[8,ind] <- uint16int8(cfgFileData[(count+22):(count+23)])#numAnlgs
      PMU.basic[9,ind] <- uint16int8(cfgFileData[(count+24):(count+25)])#numDigs
      
      count <- count+30+20*(PMU.basic[7,ind]+PMU.basic[8,ind]+PMU.basic[9,ind])+240*PMU.basic[9,ind]
    }
    
    numph <- PMU.basic[7,]
    numph <- numph[which.max(numph)]				#find the max number of all the phasors
    config.basic[8,1] <- numph
    numan <- PMU.basic[8,]
    numan <- numan[which.max(numan)]				#find the max number of all the analogs
    config.basic[9,1] <- numan
    numdg <- PMU.basic[9,]
    numdg <- numdg[which.max(numdg)]				#find the max number of all the digitals
    config.basic[10,1] <- numdg
    
    count <- 21
    dataFrameCount <- 15
    
    for(ind in 1:numPMUs) 
    {
      fmtStr <- as.integer(intToBits(PMU.basic[2,ind]))
      fmtStr <- fmtStr[16:1]
      PMU.basic[3,ind] <- fmtStr[13]									#frqFmt
      PMU.basic[4,ind] <- fmtStr[14]									#anFmt
      PMU.basic[5,ind] <- fmtStr[15]									#phFmt
      PMU.basic[6,ind] <- fmtStr[16]									#phCoord
      numPhsrs <- PMU.basic[7,ind]
      numAnlgs <- PMU.basic[8,ind]
      numDigs <- PMU.basic[9,ind]
      
      count <- count +26
      PMU.basic[10,ind] <- dataFrameCount								#pmuOffset
      dataFrameCount <- dataFrameCount+2
      
      for(ind2 in 1:numPhsrs) 
      {
        if(ind2==1 && ind==1) 	#define a 3-dimensional array to store all the phasors information
        {
          phsr.name <- matrix(,numph,numPMUs)
          phsr <- array(,dim=c(numph,numPMUs,5),dimnames = list(c(),c(),c("sigOffset",
                                                                          "phUnit","sigType","scale","scaleStr")))
        }
        phsr.name[ind2,ind] <- dec2char(cfgFileData[count:(count+15)])	#name
        phsr[ind2,ind,1] <- dataFrameCount								#sigOffset
        count <- count+16
        if (PMU.basic[5,ind]==1)  dataFrameCount <- dataFrameCount+8 else  
          dataFrameCount <- dataFrameCount+4
      }
      
      PMU.basic[11,ind] <- dataFrameCount								#frqOffset
      if (PMU.basic[3,ind]==1)  dataFrameCount <- dataFrameCount+8 else  
        dataFrameCount <- dataFrameCount+4
      
      for(ind2 in 1:numAnlgs)
      {
        if(ind2==1 && ind==1) 	#define a 3-dimensional array to store all the analogs information
        {
          anlg.name <- matrix(,numan,numPMUs)
          anlg <- array(,dim=c(numan,numPMUs,6),dimnames = list(c(),c(),c("name","sigOffset",
                                                                          "anUnit","sigType","scale","scaleStr")))
        }
        anlg.name[ind2,ind] <- dec2char(cfgFileData[count:(count+15)])	#name
        anlg[ind2,ind,1] <- dataFrameCount								#sigOffset
        count <- count+16
        if (PMU.basic[4,ind]==1)  dataFrameCount <- dataFrameCount+4 else  
          dataFrameCount <- dataFrameCount+2
      }
      
      for(ind2 in 1:numDigs)
      {
        if(ind2==1 && ind==1)	#define a 3-dimensional array to store all the digitals information
          dig <- array(,dim=c(19,numPMUs,numdg),dimnames = list(c("channel 1 name","2","3","4","5","6","7",
                                                                  "8","9","10","11","12","13","14","15","16","sigOffset","digUnit","Str"),c(),c()))
        for(ind3 in 1:16)
        {
          dig[ind3,ind,ind2] <- dec2char(cfgFileData[count:(count+15)]);#name
          count <- count+16
        }
        dig[17,ind,ind2] <- dataFrameCount;								#sigOffset
        dataFrameCount <- dataFrameCount+2;
      }
      
      for(ind2 in 1:numPhsrs)
      {
        phsr[ind2,ind,2] <- uint32int8(cfgFileData[count:(count+3)])	#phUnit
        count <- count + 4
        phUnitStr <- as.integer(intToBits(phsr[ind2,ind,2]))
        phUnitStr <- phUnitStr[32:1]
        phsr[ind2,ind,3] <- phUnitStr[8]								#sigType
        phsr[ind2,ind,4] <- bin2dec(phUnitStr[9:32])					#scale
        phsr[ind2,ind,5] <- phsr[ind2,ind,4]*10^-5						#scaleStr
      }
      
      for(ind2 in 1:numAnlgs)
      {
        anlg[ind2,ind,2] <- uint32int8(cfgFileData[count:(count+3)])	#anUnit
        count <- count + 4
        anUnitStr <- as.integer(intToBits(anlg[ind2,ind,2]))
        anUnitStr <- anUnitStr[32:1]
        anlg[ind2,ind,3] <- as.numeric(implode(anUnitStr[7:8]))			#sigType
        anlg[ind2,ind,4] <- bin2dec(phUnitStr[9:32])					#scale
        anlg[ind2,ind,5] <- anlg[ind2,ind,4]							#scaleStr
      }
      for(ind2 in 1:numDigs)
      {
        dig[18,ind,ind2] <- uint32int8(cfgFileData[count:(count+3)])	#digUnit
        count <- count + 4
        digUnitStr <- as.integer(intToBits(dig[18,ind,ind2]))
        dig[19,ind,ind2] <- implode(digUnitStr[32:1])					#Str
      }
      
      nomFrqBit <- uint16int8(cfgFileData[count:(count+1)])
      if (nomFrqBit == 0) PMU.basic[12,ind] <- 60 else					#nomFrq
        PMU.basic[12,ind] <- 50
      
      PMU.basic[13,ind] <- uint16int8(cfgFileData[(count+2):(count+3)])	#cfgCount
      count <- count + 4
    }
    
    config.basic[6,1] <- int16(cfgFileData[count:(count+1)]);			#dataRate
    if (config.basic[6,1] < 0) config.basic[6,1] <- -1/config.basic[6,1];
    
    checkSum <- uint16int8(cfgFileData[(count+2):(count+3)]);
    config.basic[7,1] <- dataFrameCount+1;								#dataFrameSize
    
    config <- list(CG=config.basic,PMU.name=PMU.name,PMU.basic=PMU.basic,PH.name=phsr.name,PH=phsr,AN.name=anlg.name,AN=anlg,DG=dig);
    return(config)
  }
  ###
  sortSOC <- function(SOC,fracSec,timeBase,dataRate) # a function to sort SOC values
  {
    timeArr <- matrix(,length(SOC),3);
    timeArr[,1] <- SOC;
    timeArr[,2] <- fracSec;
    
    a <- seq(1,(dataRate*2-2),2);
    lowLim <- c(0, a/2);
    highLim <- c(a/2, dataRate);
    
    x <- fracSec/timeBase*dataRate;
    
    SOCvals <- unique(SOC);
    
    for(ind in 1:length(SOCvals)) 
    {
      SOCloc <- which(SOC == SOCvals[ind],arr.ind = TRUE);
      tempFracSec <- fracSec[SOCloc];
      for(ind2 in 1:length(tempFracSec))
      {
        for(ind3 in 1:dataRate) 
        {
          if (x[SOCloc[ind2]] >= lowLim[ind3] && x[SOCloc[ind2]] < highLim[ind3])
            timeArr[SOCloc[ind2],3] <- ind3;
        }
      }
    }
    return(timeArr)
  }
  ###
  ############################################################################
  ###
  to.read <- file(filepath,"rb");
  #*
  # Part 1: FileVersionFlags
  #*
  FileVersionFlags <- readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little");
  fileVersion <- FileVersionFlags[1];
  c37Version <- FileVersionFlags[2];
  dataPadded <- FileVersionFlags[3];
  flags <- FileVersionFlags[4];
  flagBin <- as.integer(intToBits(flags));									    	#bit mapped flags
  
  incIniFile <- flagBin[3];									#check whether part Ini is included
  incCfgFile <- flagBin[2];									#check whether part CFG is included
  incHdrFile <- flagBin[1];									#check whether part Header is included
  #*
  # Part 2: OffsetToData
  #*
  offsetToData<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));	#the offset byte to the data part
  idx=9;									                            #document which byte that has been read
  #*
  # Part 3: HeaderFile
  #*
  hdrPtrLength<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));	#read the first 4 bytes to get the length of this part
  hdrPtr<-readBin(to.read,integer(),hdrPtrLength,size=1,signed = FALSE,endian="little");
  idx=idx+4+hdrPtrLength;
  #*
  # Part 4: Header
  #*
  hdrDataLength<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));
  hdrFileData<-readBin(to.read,integer(),hdrDataLength,size=1,signed = FALSE,endian="little");
  idx=idx+4+hdrDataLength;
  
  if(incHdrFile==1) hdrData <- hdrFileData else	    					#check whether Header part exists
    if(hdrPtrLength>0 && any(names(df) == "hdrPtr")) 					#read header part
      hdrData <- c() else hdrData <- c();
  #*
  #Part 5: CFG2File
  #*
  cfgPtrLength<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));
  cfgPtr<-readBin(to.read,integer(),cfgPtrLength,size=1,signed = FALSE,endian="little");
  idx=idx+4+cfgPtrLength;
  #*
  # Part 6: CFG2
  #*
  cfgDataLength<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));
  cfgFileData<-readBin(to.read,integer(),cfgDataLength,size=1,signed = FALSE,endian="little");
  idx=idx+4+cfgDataLength;
  
  if(incCfgFile==1) config<-readConfigFrame(cfgFileData) else			#check whether CFG2 part exists
    if(cfgPtrLength>0 &&  any(names(df) == "cfgPtr")) 
      pdc <- readConfigFrame(cfgPtr) else config <- c();
  #*
  # Part 7: IniFile
  #*
  iniPtrLength<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));
  iniPtr<-readBin(to.read,integer(),iniPtrLength,size=1,signed = FALSE,endian="little");
  idx=idx+4+iniPtrLength;
  #*
  # Part 8: Ini
  #*
  iniDataLength<-hex2dec(readBin(to.read,integer(),4,size=1,signed = FALSE,endian="little"));
  iniFileData<-readBin(to.read,integer(),iniDataLength,size=1,signed = FALSE,endian="little");
  idx=idx+4+iniDataLength;
  
  if(incIniFile==1) iniData <- iniFileData else	    	#check whether Ini part exists
    if(iniPtrLength>0 && any(names(df) == "iniPtr")) 	#read ini part
      iniData <- c() else iniData <- c();
  #*
  # Part 9: Data
  #*
  dataFrameSize <- config$CG[7,1];					    #read the dataFrameSize from list config
  timeBase <- config$CG[4,1];							#read the timeBase from list config
  dataRate <- config$CG[6,1];							#read the dataRate from list config
  
  rawArr <- readBin(to.read,raw(),dataFrameSize*3600*60,size=1,endian="little");	#read all the data out
  close(to.read);										#close the connection to the file
  index <- length(rawArr)/dataFrameSize;				#calculate the loop numbers
  rawArr <- array(rawArr,dim=c(dataFrameSize,index));	#change the data into a 2-dimension matrix
  
  tempData <- rawArr[7:10,];
  data.SOC <- readBin(tempData,integer(),index,4,endian="big");
  tempData <- rawArr[11:14,];
  data.fracSec <- readBin(tempData,integer(),index,4,endian="big");
  data.timeArr <- sortSOC(data.SOC, data.fracSec, timeBase, dataRate); 	#use the sortSOC function
  
  SOCday <- as.POSIXct(data.timeArr[1,1],origin="1970-01-01",tz="GMT");	#calculate the recording date from SOC
  options(digits.secs=6);
  timeSeq <- data.timeArr[,2]/timeBase;		      		#get the time sequence from timeArr
  e <- -1;
  for(ind in 1:index) 
  {
#    if(data.timeArr[ind,3] == 1)	e <- e+1;
    if(data.timeArr[ind,3] == 1 & !is.na(data.timeArr[ind,3])) {
      e <- e+1
    } else {
      break
    }
    timeSeq[ind] <- e+timeSeq[ind];
  }
  timeSeq <- timeSeq+SOCday;
  
  numph <- config$CG[8,1];								#read the max number of phasors
  numan <- config$CG[9,1];								#read the max number of analogs
  numdg <- config$CG[10,1];								#read the max number of digitals
  numPMUs <- config$CG[5,1];							#read the numPMUs from list config
  
  for(ind in 1:numPMUs)	      							#use a loop to all the PMUs
  {
    if (ind==1) #define 3 matrix to store stat frq rocof(rate of change of frequency)
    {
      pmu.stat <- matrix(,index,numPMUs);
      pmu.freq <- matrix(,index,numPMUs);
      pmu.rocof <- matrix(,index,numPMUs);
    }
    pmuOffset <- config$PMU.basic[10,ind];				#read the pmuOffset from list config
    tempData <- rawArr[pmuOffset:(pmuOffset+1),];
    pmu.stat[,ind] <- readBin(tempData,integer(),index,2,signed=FALSE,endian="big");
    
    numPhsrs <- config$PMU.basic[7,ind];				#read the numPhsrs from list config
    for(ind2 in 1:numPhsrs)   							#use a loop to read all the Phasors
    {
      if (ind2==1 && ind==1) 							#define 4 3-dimensional array for magnitude, angle, real and imaginary part
      {
        phsr.mag <- array(,dim=c(index,numPMUs,numph));
        phsr.ang <- array(,dim=c(index,numPMUs,numph));
        phsr.real <- array(,dim=c(index,numPMUs,numph));
        phsr.imag <- array(,dim=c(index,numPMUs,numph));
      }
      phsrOffset <- config$PH[ind2,ind,1];				#read the phsrOffset from list config
      phFmt <- config$PMU.basic[5,ind];					#read the phasor Format from list config
      phCoord <- config$PMU.basic[6,ind];				#read the phasor Coordinate from list config
      if (phFmt==1 && phCoord==1)	#read the magnitude and angle of the phasor according to its phasor format and coordinates
      {
        tempData <- rawArr[phsrOffset:(phsrOffset+3),];
        tempData2 <- rawArr[(phsrOffset+4):(phsrOffset+7),];
        phsr.mag[,ind,ind2] <- readBin(tempData,double(),index,4,endian="big");
        phsr.ang[,ind,ind2] <- readBin(tempData2,double(),index,4,endian="big")*180/pi;
      } else 
        if (phFmt==1 && phCoord==0) 
        {
          tempData <- rawArr[phsrOffset:(phsrOffset+3),];
          tempData2 <- rawArr[(phsrOffset+4):(phsrOffset+7),];
          phsr.real[,ind,ind2] <- readBin(tempData,double(),index,4,endian="big");
          phsr.imag[,ind,ind2] <- readBin(tempData2,double(),index,4,endian="big");
          phsr.mag[,ind,ind2] <- cart2pol(phsr.real,phsr.imag)[1];
          phsr.ang[,ind,ind2] <- cart2pol(phsr.real,phsr.imag)[2];
        } else 
          if (phFmt==0 && phCoord==1) 
          {
            tempData <- rawArr[phsrOffset:(phsrOffset+1),];
            scale <- config$PH[ind2,ind,4];				#read scale of the magnitude from list config
            tempData2 <- rawArr[(phsrOffset+2):(phsrOffset+3),];
            phsr.mag[,ind,ind2] <- readBin(tempData,double(),index,4,endian="big");
            phsr.ang[,ind,ind2] <- readBin(tempData2,double(),index,4,endian="big")*180/pi*0.0001;
          } else 
          {
            tempData <- rawArr[phsrOffset:(phsrOffset+1),];
            scale <- config$PH[ind2,ind,4];				#read scale of the magnitude from list config
            tempData2 <- rawArr[(phsrOffset+2):(phsrOffset+3),];
            phsr.real[,ind,ind2] <- readBin(tempData,double(),index,4,endian="big");
            phsr.imag[,ind,ind2] <- readBin(tempData2,double(),index,4,endian="big");
            phsr.mag[,ind,ind2] <- cart2pol(phsr.real,phsr.imag)[1]*scale;
            phsr.ang[,ind,ind2] <- cart2pol(phsr.real,phsr.imag)[2];
          }
    }
    
    numAnlgs <- config$PMU.basic[8,ind]; 				#read the number of analogs from list config
    for(ind2 in 1:numAnlgs)   							#use a loop to read all the analogs
    {
      if (ind2==1 && ind==1) 							#define a 3-dimension array to store all the analogs value
        anlg.val <- array(,dim=c(index,numPMUs,numan));
      anlgOffset <- config$AN[ind2,ind,1];				#read the offset of analogs from list config
      anFmt <- config$PMU.basic[4,ind];					#read the format of analogs from list config
      if (anFmt==1) 									#read the values of the analogs according to its format 
      {
        tempData <- rawArr[anlgOffset:(anlgOffset+3),];
        anlg.val[,ind,ind2] <- readBin(tempData,double(),index,4,endian="big");
      } else 
      {
        tempData <- rawArr[anlgOffset:(anlgOffset+1),];
        scale <- config$AN[ind2,ind,4];					#read the scale of value from list config
        anlg.val[,ind,ind2] <- readBin(tempData,integer(),index,2,signed=FALSE,endian="big")*scale;
      }
    }
    
    numDigs <- config$PMU.basic[9,ind];					#read the number of digitals from list config
    for(ind2 in 1:numDigs) 	  							#use a loop to read all the digitals
    {
      if (ind2==1 && ind==1)							#define a 3-dimension array to store all the analogs value
        dig.val <- array(,dim=c(index,numPMUs,numdg))	
      digOffset <- as.numeric(config$DG[17,ind,ind2]);				#read the offset of digitals from list config
      tempData <- rawArr[digOffset:(digOffset+1),];
      dig.val[,ind,ind2] <- readBin(tempData,integer(),index,2,signed=FALSE,endian="big");
    }
    
    frqOffset <- config$PMU.basic[11,ind];				#read the offset of frequency from list config
    frqFmt <- config$PMU.basic[3,ind];				  	#read the format of frequency from list config
    if (frqFmt==1) 										#read the values of the frequency and rocof according to its format 
    {
      chkOffset <- frqOffset+8;
      tempData <- rawArr[frqOffset:(frqOffset+3),];
      tempData2 <- rawArr[(frqOffset+4):(frqOffset+7),];
      pmu.freq[,ind] <- readBin(tempData,double(),index,4,endian="big");
      pmu.rocof[,ind] <- readBin(tempData2,double(),index,4,endian="big");
    } else 
    {
      tempData <- rawArr[frqOffset:(frqOffset+1),];
      tempData2 <- rawArr[(frqOffset+2):(frqOffset+3),];
      nomFrq <- config$PMU.basic[12,ind];				#read the from list config
      chkOffset <- frqOffset+4;
      pmu.freq[,ind] <- readBin(tempData,integer(),index,2,signed=FALSE,endian="big")*0.001+nomFrq;
      pmu.rocof[,ind] <- readBin(tempData2,integer(),index,2,signed=FALSE,endian="big")*0.01;
    }
  }
  
  col <- 3*numPMUs+2*sum(config$PMU.basic[7,])+sum(config$PMU.basic[8,])+sum(config$PMU.basic[9,]);	  #define a index to calculate the dimension of PMU matrix
  PMU <- matrix(,index,col);							#define a matrix to store all the PMU data
  nind <- 0;											#define a index for column
  
  colname <- matrix(,1,col);							#define a matrix for column names	
  for(ind in 1:numPMUs) 
  {
    pmu.name <- config$PMU.name[1,ind];				 	#find the pmu name
    numPhsrs <- config$PMU.basic[7,ind];
    numAnlgs <- config$PMU.basic[8,ind];
    numDigs <- config$PMU.basic[9,ind];
    nind <- nind+1;
    colname[nind] <- paste(pmu.name,"stat",sep=".");
    PMU[,nind] <- pmu.stat[,ind];
    for(ind2 in 1:numPhsrs) 							#use a loop for every phasor
    {
      phsr.name <- config$PH.name[ind2,ind];
      nind <- nind+2;
      colname[nind-1] <- paste(pmu.name,phsr.name,"MAG",sep=".");
      PMU[,(nind-1)] <- phsr.mag[,ind,ind2];
      colname[nind] <- paste(pmu.name,phsr.name,"ANG",sep=".");
      PMU[,nind] <- phsr.ang[,ind,ind2];
    }
    for(ind2 in 1:numAnlgs) 							#use a loop for every analogs
    {
      anlg.name <- config$AN.name[ind2,ind];
      nind <- nind+1;
      colname[nind] <- paste(pmu.name,anlg.name,sep=".");
      PMU[,nind] <- anlg.val[,ind,ind2];
    }
    for(ind2 in 1:numDigs)  							#use a loop for every digitals
    {
      dig.name <- config$DG[1,ind,ind2];				#15 channels omitted
      nind <- nind+1;
      colname[nind] <- paste(pmu.name,dig.name,sep=".");
      PMU[,nind] <- dig.val[,ind,ind2];
    }
    colname[nind+1] <- paste(pmu.name,"freq",sep=".");
    PMU[,(nind+1)] <- pmu.freq[,ind];
    colname[nind+2] <- paste(pmu.name,"rocof",sep=".");
    PMU[,(nind+2)] <- pmu.rocof[,ind];
    nind <- nind+2;
  }
  rownames(PMU) <- sprintf("%s",timeSeq);  				#print the date and time on the left side
  colnames(PMU) <- colname;
  
  PMU
}