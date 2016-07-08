rm(list = setdiff(ls(), lsf.str()))

library(signal)
library(snowfall)

library(bpaFUN) # Just for DQfilter
library(FODetection)

# source("D:\\DISAT_PDAT\\FOdet\\readPDAT_FODet_function.r")


### User chosen parameters

# Number of CPUs to use in parallel processing
NumCPUsPer = 4
NumCPUsSC = 4

# To keep the amount of Rdata that is stored on the computer reasonable, this script can delete past files.
# Past R data files are only deleted if DeletePastRdata == TRUE
# NumDaysRdataToKeep specifies how much Rdata to store. For example, if DeletePastRdata is TRUE and
# NumDaysRdataToKeep=365, whenver an Rdata file is loaded the Rdata file from one year previous
# will be deleted.
DeletePastRdata = TRUE
NumDaysRdataToKeep = 18*30

sample.rate=60

# How long to sleep and number of times to sleep when no data beyond the current minute is available
# If the number of sleeps is exceeded, all processing will stop
sleepTimeNoDataAvail = 60
numSleepsNoDataAvail = 60*2

# How long to sleep and number of times to sleep when data beyond the current minute is available
# If the number of sleeps is exceeded, the algorithm will move beyond the current minute to the next available minute of data
sleepTimeDataAvail = 60
numSleepsDataAvail = 60*2

# The start and end times (inclusive) to perform the analysis
#
# Make sure to take into account how much data needs to be stored before the
# algorithms will start processing, i.e., if N=10*60*sample.rate (defined
# later in the initialization) and StartTime is "2016-01-29 00:00:00" then the
# first detection results will be recorded for "2016-01-29 00:09:00".
#
# If StartTime is NULL, the script will process data as it becomes available
# starting one hour before the current system time.
# If EndTime is NULL, the code will continue to run until stopped manually (hit
# escape in the R Console window).
#
StartTime = NULL
EndTime = NULL
# StartTime = "2015-10-18 00:00:00"
# EndTime = "2015-10-18 23:59:00"

# Filepath to where Rdata files converted from PDAT are
FilePathBase = "D:/DISAT_PDAT/Rdata_PDAT/"

# Filepath to where raw pdat files are
FilePathBaseRaw = "J:/"

# Filepath to save outputs from test
SavePathBase = "D:/DISAT_PDAT/Output_FOdet/"

# These are the names of the MW and MVAR channels to be analyzed
# Multiple channels from a single PMU can be used, but plots will suffer. Also keep the computational burden in mind.
# If MW and MVAR (abbreviated MV in PMU naming convention) for the same signal are included, they will automatically
# be converted to apparent power and denoted with an S in place of the MW and MV in the channel name.
MultiChanNamesInit = c(
  "ALSN500A1SA.A500NAPAVINE_1MW" ,
  "ALVY500A1SA.A500MARION___1MW" ,
  "ASHE500A1SA.A500MARION___1MW" ,
  "BELL230A1SA.A230WESTSIDE_1MW" ,
  "BGED500A1SA.A500CELILO___1MW" ,
  "CPJK500A1SA.A500GRIZZLY__1MW" ,
  "CEFE500A1SA.A500BANK1____1MW" ,
  "CHJO230A1SA.A230CHFJO_PH_1MW" ,
  "CUST500A1SA.A500MONROE___2MW" ,
  "ECOL500A1SA.A500SCHULTZ__1MW" ,
  "GARR500A1SA.A500TAFT_____1MW" ,
  "GRTW230A1SA.A230BELL_____1MW" ,
  "GCFI500A1SA.A500GEN19____1MW" ,
  "GRIZ500A1SA.A500BUCKLEY__1MW" ,
  "JDTW230A1SA.A230BIGLCAYN_1MW" ,
  "JDAY500A1SA.A500SLATT____1MW" ,
  "KEEL230A1SA.A230RIVERGAT_1MW" ,
  "LOMO500A1SA.A500LOMON_PH_1MW" ,
  "MALN500A1SA.A500GRIZZLY__2MW" ,
  "MARN500A1SA.A500PEARL____1MW" ,
  "MCNY230A1SA.A230ROUNDUP__1MW" ,
  "MONE500A1SA.A500BANK1____1MW" ,
  "NBON230A1SA.A230BONNVILL_1MW" ,
  "OSTD500A1SA.A500TROUTDAL_1MW" ,
  "PAUL500A1SA.A500CENT_STM_2MW" ,
  "PERL500A1SA.A500MARION___1MW" ,
  "RKCR230A1SA.A230IMRIE____1MW" ,
  "SHUL500A1SA.A500REACG1___1MW" ,
  "SLAT230A1SA.A230SHEPDSFT_1MW" ,
  "SUML500A1SA.A500MALIN____1MW" ,
  "TROU230A1SA.A230BIG_EDDY_1MW" ,
  "WAUT500A1SA.A500SCHULTZ__1MW" ,

  "ALSN500A1SA.A500NAPAVINE_1MV" ,
  "ALVY500A1SA.A500MARION___1MV" ,
  "ASHE500A1SA.A500MARION___1MV" ,
  "BELL230A1SA.A230WESTSIDE_1MV" ,
  "BGED500A1SA.A500CELILO___1MV" ,
  "CPJK500A1SA.A500GRIZZLY__1MV" ,
  "CEFE500A1SA.A500BANK1____1MV" ,
  "CHJO230A1SA.A230CHFJO_PH_1MV" ,
  "CUST500A1SA.A500MONROE___2MV" ,
  "ECOL500A1SA.A500SCHULTZ__1MV" ,
  "GARR500A1SA.A500TAFT_____1MV" ,
  "GRTW230A1SA.A230BELL_____1MV" ,
  "GCFI500A1SA.A500GEN19____1MV" ,
  "GRIZ500A1SA.A500BUCKLEY__1MV" ,
  "JDTW230A1SA.A230BIGLCAYN_1MV" ,
  "JDAY500A1SA.A500SLATT____1MV" ,
  "KEEL230A1SA.A230RIVERGAT_1MV" ,
  "LOMO500A1SA.A500LOMON_PH_1MV" ,
  "MALN500A1SA.A500GRIZZLY__2MV" ,
  "MARN500A1SA.A500PEARL____1MV" ,
  "MCNY230A1SA.A230ROUNDUP__1MV" ,
  "MONE500A1SA.A500BANK1____1MV" ,
  "NBON230A1SA.A230BONNVILL_1MV" ,
  "OSTD500A1SA.A500TROUTDAL_1MV" ,
  "PAUL500A1SA.A500CENT_STM_2MV" ,
  "PERL500A1SA.A500MARION___1MV" ,
  "RKCR230A1SA.A230IMRIE____1MV" ,
  "SHUL500A1SA.A500REACG1___1MV" ,
  "SLAT230A1SA.A230SHEPDSFT_1MV" ,
  "SUML500A1SA.A500MALIN____1MV" ,
  "TROU230A1SA.A230BIG_EDDY_1MV" ,
  "WAUT500A1SA.A500SCHULTZ__1MV"
)


# The following parameters are for the detection algorithms.
# They are described in detail in the user's manual.
# Changes to these values should be made with caution.

N = 10*60*sample.rate  # Amount of data stored. Needs to be an integer multiple of 60*sample.rate, the length of each PDAT file

# Parameters for both algorithms
K0 = N
FreqTol = 0.05

# minimum and maximum frequencies to look for forced oscillations
fmin = 0.1
fmax = 18

# Frequency bin stuff
f = sample.rate*(0:(K0-1))/K0
OmegaB = which((f>fmin) & (f<fmax))
f = f[OmegaB]
B = length(f)

# Periodogram parameters
Nper = N
Pfa = rep(0.000001,B)
win_Per = hamming(Nper)
win_GMSC = hamming(Nper/3)
Noverlap_GMSC = length(win_GMSC)/2
Nmed = 37

# SC parameters
Nsc = 50*sample.rate
D = 10*sample.rate
NumDelay = 3
gam_SC = 3
win_SC = hamming(Nsc/5)
Noverlap_SC = length(win_SC)/2


### END user choices




### Initialization

# Gets all the necessary information to be able to replace MW and MV signals from the same channel with apparent power
MultiChanNames = MultiChanNamesInit
MultiChanNamesToS = c()
for (chan in 1:length(MultiChanNamesInit)) {
  ChanNameNoUnit = substr(MultiChanNamesInit[chan],1,26)
  if (substr(MultiChanNamesInit[chan],27,28) == "MW") {
    if (length(which(MultiChanNamesInit == paste(ChanNameNoUnit,"MV",sep=""))) > 0) {
      MultiChanNamesToS = c(MultiChanNamesToS, ChanNameNoUnit)

      MultiChanNames[grepl(ChanNameNoUnit,MultiChanNames)] = paste(ChanNameNoUnit,"S",sep="")
    }
  }
}
MultiChanNames = unique(MultiChanNames)

NumChan = length(MultiChanNames)

# If the StartTime was set to NULL, use the current system time minus one hour
if (is.null(StartTime)) {
  StartTime = toString(as.POSIXlt(Sys.time(), tz="GMT")-3600)
  StartTime = paste(substr(StartTime,1,17),"00",sep="")
}

# Convert string to the POSIXlt data type for dates
CurrentTime = as.POSIXlt(StartTime)
if (!is.null(EndTime)) {
  EndTime = as.POSIXlt(EndTime)
}

PreviousTime = CurrentTime
PreviousTime$min = PreviousTime$min-1
# R is brilliant and will store minutes beyond 59 while leaving the hours in place in the previous command. It will still display correctly.
# This line makes it so that the value is stored as a meaningful date. This is important for code later on.
PreviousTime = as.POSIXlt(toString(PreviousTime))

SigAll = matrix(0,NumChan,0)

countNoDataAvail = 0
countDataAvail = 0
while(countNoDataAvail < numSleepsNoDataAvail) {
  # The file path to the current minute
  # Produces the portion of the file path with form "2014/04/01/WISPDitt_" (folder path plus the first part of the file name)
  FilePath1 = strftime(CurrentTime, format="%Y/%m/%d/WISPDitt_")
  # Produces the portion of the file path with form "20140401_085700.Rdata" (the rest of the file name)
  FilePath2 = strftime(CurrentTime, format="%Y%m%d_%H%M%S.Rdata")
  # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/WISPDitt_20140401_085700.Rdata"
  FilePath = paste(FilePathBase,FilePath1,FilePath2,sep="")
  OutPath = paste(FilePathBase,strftime(CurrentTime, format="%Y/%m/%d"),sep="")
  
  # The file path to the current minute of raw pdat data
  # Produces the portion of the file path with form "2014/140401/WISPDitt_" (folder path plus the first part of the file name)
  FilePathRaw1 = strftime(CurrentTime, format="%Y/%y%m%d/WISPDitt_")
  # Produces the portion of the file path with form "20140401_085700.Rdata" (the rest of the file name)
  FilePathRaw2 = strftime(CurrentTime, format="%Y%m%d_%H%M%S.pdat")
  # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/WISPDitt_20140401_085700.Rdata"
  FilePathRaw = paste(FilePathBaseRaw,FilePathRaw1,FilePathRaw2,sep="")

  # Determine if the current minute is available. If not, check to see if a future minute is already available.
  # If a future minute is available, the waiting strategy is different
  if (file.exists(FilePathRaw)) {
    # Current minute is available
    
    tall = Sys.time()
    tload = NA
    tPer = NA
    tSC = NA

    tload = Sys.time()
    
    # If the Rdata file is not available, create it from the pdat file
    # Convert from pdat to RData
    if (!file.exists(FilePath)) {
      # Wait one second to make sure PDAT has enough time to be save
      Sys.sleep(1)
      input = c(FilePathRaw,OutPath)
      names(input) <- c("filename","outfolder")
      readPDAT(input)
    }

    # This shouldn't fail since I just created it
    # I'm leaving it this way, with the following if statement for redundancy    
    try(load(FilePath),silent=TRUE)

    if (!is.element("data",ls())) {
      # The file didn't load. In case it was being written at the time wait
      # 3 seconds and try again. If it still doesn't work, the file will be
      # skipped over later in the code.
      Sys.sleep(3)
      try(load(FilePath),silent=TRUE)
    }
    tload = round(difftime(Sys.time(),tload,units="secs"),2)

    if (DeletePastRdata == TRUE) {
      DeleteTime = CurrentTime
      DeleteTime$mday = DeleteTime$mday-round(NumDaysRdataToKeep,0)
      # This command simply ensures that DeleteTime is stored as expected
      # For example, that the year is adjusted rather than storing a negative day
      DeleteTime = as.POSIXlt(toString(DeleteTime))

      # The file path to the Rdata file that should be deleted
      # Produces the portion of the file path with form "2014/04/01/WISPDitt_" (folder path plus the first part of the file name)
      FilePath1 = strftime(DeleteTime, format="%Y/%m/%d/WISPDitt_")
      # Produces the portion of the file path with form "20140401_085700.Rdata" (the rest of the file name)
      FilePath2 = strftime(DeleteTime, format="%Y%m%d_%H%M%S.Rdata")
      # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/WISPDitt_20140401_085700.Rdata"
      DeleteFilePath = paste(FilePathBase,FilePath1,FilePath2,sep="")

      # If it exists, delete the old Rdata file
      if (file.exists(DeleteFilePath)) {
        file.remove(DeleteFilePath)
      }
    }

    # Produces the portion of the file path with form "2014/04/01/DetResults_" (folder path plus the first part of the file name)
    SavePath1 = strftime(CurrentTime, format="%Y/%m/%d/DetResults_")
    # Produces the portion of the file path with form "20140401_085700" (the rest of the file name)
    SavePath2 = strftime(CurrentTime, format="%Y%m%d_%H%M%S")
    # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/DetResults_20140401_085700_SC.Rdata" for SC results
    SavePathSC = paste(SavePathBase,SavePath1,SavePath2,"_SC.Rdata",sep="")
    # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/DetResults_20140401_085700_Per.Rdata" for periodogram results
    SavePathPer = paste(SavePathBase,SavePath1,SavePath2,"_Per.Rdata",sep="")

    # Set up save path for the daily summaries
    # Same for each minute in a day, but the extra computation here is worth not trying to code around all the different
    # things that could happen to determine whether or not it needs to be run.
    # Produces the portion of the file path with form "2014/04/01/DailyDet_" (folder path plus the first part of the file name)
    SavePath1 = strftime(CurrentTime, format="%Y/%m/%d/DailyDet_")
    # Produces the portion of the file path with form "20140401" (the rest of the file name)
    SavePath2 = strftime(CurrentTime, format="%Y%m%d")
    # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/DailyDet_20140401_SC.Rdata" for SC results
    SavePathDailySC = paste(SavePathBase,SavePath1,SavePath2,"_SC.Rdata",sep="")
    # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/DailyDet_20140401_Per.Rdata" for periodogram results
    SavePathDailyPer = paste(SavePathBase,SavePath1,SavePath2,"_Per.Rdata",sep="")

    # Set up save path for the daily most significant event variables
    # Same for each minute in a day, but the extra computation here is worth not trying to code around all the different
    # things that could happen to determine whether or not it needs to be run.
    # Produces the portion of the file path with form "2014/04/01/DailyDet_" (folder path plus the first part of the file name)
    SavePath1 = strftime(CurrentTime, format="%Y/%m/DailyMSE/MSE_")
    # Produces the portion of the file path with form "20140401" (the rest of the file name)
    SavePath2 = strftime(CurrentTime, format="%Y%m%d")
    # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/DailyDet_20140401_SC.Rdata" for SC results
    SavePathMSEsc = paste(SavePathBase,SavePath1,SavePath2,"_SC.Rdata",sep="")
    # Full file path has the form "//olympus/projects/disat/Rdata_PDAT/2014/04/01/DailyDet_20140401_Per.Rdata" for periodogram results
    SavePathMSEper = paste(SavePathBase,SavePath1,SavePath2,"_Per.Rdata",sep="")

    if (!file_test("-d",paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/%d"),sep=""))) {
    # if (!dir.exists(path = paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/%d"),sep=""))) {
      # This minute is the first in a new day (no directory yet)

      # Create the directories to store the results from this day
      dir.create(path = paste(SavePathBase,strftime(CurrentTime, format="%Y/"),sep=""), showWarnings = FALSE)
      dir.create(path = paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/"),sep=""), showWarnings = FALSE)
      dir.create(path = paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/%d"),sep=""), showWarnings = FALSE)
    }

    if (!file_test("-d",paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/DailyMSE"),sep=""))) {
    # if (!dir.exists(path = paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/DailyMSE"),sep=""))) {
      # Create the directory to store the daily most significant events
      dir.create(path = paste(SavePathBase,strftime(CurrentTime, format="%Y/%m/DailyMSE"),sep=""), showWarnings = FALSE)
    }


    if (!is.element("data",ls())) {
      # Data wasn't loaded either because the file is corrupted or because the connection was lost or the file was deleted since the directory was checked successfully.
      print("")
      warning(paste("File ", FilePath, " exists but could not be loaded properly."), call.=FALSE)
      data = matrix(NA,NumChan,60*sample.rate)
    }
    else {
      print("")
      print(FilePath)
      print("data was loaded")

      ## run the data quality filter
      data <- DQfilter(data=data/10000)$data

      # This index is true where the channel name in MultiChanNamesInit is not in the current dataset
      NoChan = !is.element(MultiChanNamesInit,colnames(data))
      if (sum(NoChan) > 0) {
        warning(paste("The following channels are requested in MultiChanNamesInit but are not included in the current file: ",paste(MultiChanNamesInit[NoChan],collapse=" ")),call.=FALSE)

        # Add NA columns to data for each of the requested channels that aren't in data
        # These will be ignored by the algorithms, but it prevents errors by keeping dimensions correct
        dataNA = matrix(NA,dim(data)[1],sum(NoChan))
        colnames(dataNA) = MultiChanNamesInit[NoChan]
        data = cbind(data,dataNA)
      }

      # Find where the MW and MVAR signals for the same channel are requested and replace with apparent power
      if (length(MultiChanNamesToS) > 0) {
        dataS = matrix(NA,dim(data)[1],length(MultiChanNamesToS))
        colnames(dataS) = paste(MultiChanNamesToS,"S",sep="")
        for (chan in 1:length(MultiChanNamesToS)) {
          dataS[,chan] = sqrt(data[,paste(MultiChanNamesToS[chan],"MW",sep="")]^2 + data[,paste(MultiChanNamesToS[chan],"MV",sep="")]^2)
        }
        data = cbind(data,dataS)
      }

      data = t(data[,MultiChanNames])  # Transpose so that the dimensions are NumSig x N, which is the way the detection algorithm functions require
    }
    
    data[data==0] = NA

    # Need to account for minutes that were skipped over because data wasn't available
    NumMinsSkipped = as.numeric(difftime(CurrentTime,PreviousTime,units="mins"))-1
    if (NumMinsSkipped > 0) {
      # Minutes were skipped over because data wasn't available
      # Only channels with no NA values are analyzed, so nothing that came before the skip can be used
      # Reinitialize SigAll
      SigAll = matrix(0,NumChan,0)
    }

    SigAll = cbind(SigAll,data)

    # Keep only N samples (unless SigAll doesn't have N samples yet, then leave alone)
    SigAll = SigAll[,max(c(1,(dim(SigAll)[2]-N+1))):dim(SigAll)[2],drop=FALSE]

    if (dim(SigAll)[2] >= Nper) { # Enough data to use Periodogram detector
    # if (dim(SigAll)[2] >= (Nsc+D*(NumDelay-1))) { # Enough data to use SC detector
      SigAllSC = SigAll[,(dim(SigAll)[2]-Nsc-D*(NumDelay-1)+1):dim(SigAll)[2],drop=FALSE]  # Grab most recent Nsc samples

      # Only keep channels with no NA values
      KillChanSC = which(rowSums(is.na(SigAllSC)) > 0)
      KeepChanSC = which(rowSums(is.na(SigAllSC)) == 0)
      NumChanKeepSC = length(KeepChanSC)

      for (SigIdx in KeepChanSC) {
        SigAllSC[SigIdx,] = SigAllSC[SigIdx,] - mean(SigAllSC[SigIdx,])  # Remove the mean
      }

      if (NumChanKeepSC > 0) {  # Have to have at least one good channel
        tSC = Sys.time()
        SCout = MultiChanSC_GMSC_fast_FreqRef_MP(SigAllSC[KeepChanSC,,drop=FALSE],win_SC,Noverlap_SC,K0,sample.rate,f,OmegaB,Nsc,D,NumDelay,gam_SC,FreqTol,NumCPUsSC)
        tSC = round(difftime(Sys.time(),tSC,units="secs"),2)

        # Save detection results for this minute
        save(SCout,f,MultiChanNames,KeepChanSC,KillChanSC, file=SavePathSC)

        # DailyDetSC and DailyDetFreqSC hold the spectral coherence method detection results that summarize each day
        # The frequency vectors are contained within the data frames in the lists, but having them as vectors makes access much easier
        # If the Rdata file containing DailyDetSC and DailyDetFreqSC exists, load it
        # If it doesn't exist yet, set DailyDetSC and DailyDetFreqSC to empty.
        if (file.exists(path = SavePathDailySC)) {
          # if (dir.exists(path = SavePathDailySC)) {
          load(SavePathDailySC)

          # Removes entries with peaks occurring after or coincidentally with CurrentTime
          # Adjusts EndTime of entries where EndTime was after or coincidental with CurrentTime
          CleanOut = CleanDailyDet(DailyDetSC,DailyDetFreqSC,CurrentTime)
          DailyDetSC = CleanOut$DailyDet
          DailyDetFreqSC = CleanOut$DailyDetFreq
        } else {
          DailyDetFreqSC = c()
          DailyDetSC = list()
        }

        # Update DailyDetSC and DailyDetFreqSC, which summarizes detections for the day
        NewDailyDet = UpdateDailyDet(DailyDetSC,DailyDetFreqSC,SCout$FOfreq,SCout$FOevidence,FreqTol,CurrentTime,PreviousTime,MultiChanNames[KeepChanSC])
        DailyDetSC = NewDailyDet$DailyDet
        DailyDetFreqSC = NewDailyDet$DailyDetFreq

        # Save the detection summaries for this day (saved each minute so that they can be accessed in GUI)
        save(DailyDetSC, DailyDetFreqSC, file=SavePathDailySC)


        # Find most significant event for the day and save in directory for this month (saved each minute so that they can be accessed in GUI)
        MSE = DailyMSE(DailyDetSC,DailyDetFreqSC)
        save(MSE, file=SavePathMSEsc)
        rm(DailyDetSC,DailyDetFreqSC)
      }
    }

    if (dim(SigAll)[2] >= Nper) { # Enough data to use Periodogram detector
      SigAllPer = SigAll[,(dim(SigAll)[2]-Nper+1):dim(SigAll)[2],drop=FALSE]  # Grab most recent Nper samples

      # Only keep channels with no NA values
      KillChanPer = which(rowSums(is.na(SigAllPer)) > 0)
      KeepChanPer = which(rowSums(is.na(SigAllPer)) == 0)
      NumChanKeepPer = length(KeepChanPer)

      for (SigIdx in KeepChanPer) {
        SigAllPer[SigIdx,] = SigAllPer[SigIdx,] - mean(SigAllPer[SigIdx,])  # Remove the mean
      }

      if (NumChanKeepPer > 0) {  # Have to have at least one good channel
        # Periodogram thresholds
        gam_indy = qchisq(1-Pfa/B,2*NumChanKeepPer)
        gam_ident = NumChanKeepPer*qchisq(1-Pfa/B,2)

        tPer = Sys.time()
        PerOut = MultiChanPer_GMSC_fast_FreqRef(SigAllPer[KeepChanPer,,drop=FALSE],win_Per,win_GMSC,Noverlap_GMSC,K0,sample.rate,f,OmegaB,gam_indy,gam_ident,Nmed,FreqTol,NumCPUsPer)
        tPer = round(difftime(Sys.time(),tPer,units="secs"),2)

        # Save detection results for this minute
        save(PerOut,f,MultiChanNames,KeepChanPer,KillChanPer, file=SavePathPer)

        # DailyDetPer and DailyDetFreqPer hold the periodogram method detection results that summarize each day
        # The frequency vectors are contained within the data frames in the lists, but having them as vectors makes access much easier
        # If the Rdata file containing DailyDetPer and DailyDetFreqPer exists, load it
        # If it doesn't exist yet, set DailyDetPer and DailyDetFreqPer to empty.
        if (file.exists(path = SavePathDailyPer)) {
          # if (dir.exists(path = SavePathDailyPer)) {
          load(SavePathDailyPer)

          # Removes entries with peaks occurring after or coincidentally with CurrentTime
          # Adjusts EndTime of entries where EndTime was after or coincidental with CurrentTime
          CleanOut = CleanDailyDet(DailyDetPer,DailyDetFreqPer,CurrentTime)
          DailyDetPer = CleanOut$DailyDet
          DailyDetFreqPer = CleanOut$DailyDetFreq
        } else {
          DailyDetFreqPer = c()
          DailyDetPer = list()
        }

        # Update DailyDetPer and DailyDetFreqPer, which summarizes detections for the day
        NewDailyDet = UpdateDailyDet(DailyDetPer,DailyDetFreqPer,PerOut$FOfreq,PerOut$FOevidence,FreqTol,CurrentTime,PreviousTime,MultiChanNames[KeepChanPer])
        DailyDetPer = NewDailyDet$DailyDet
        DailyDetFreqPer = NewDailyDet$DailyDetFreq

        # Save the detection summaries for this day (saved each minute so that they can be accessed in GUI)
        save(DailyDetPer, DailyDetFreqPer, file=SavePathDailyPer)


        # Find most significant event for the day and save in directory for this month (saved each minute so that they can be accessed in GUI)
        MSE = DailyMSE(DailyDetPer,DailyDetFreqPer)
        save(MSE, file=SavePathMSEper)
        rm(DailyDetPer,DailyDetFreqPer)
      }
    }

    rm(data)

    # The current minute has been analyzed, so update PreviousTime and increment the CurrentTime so that the next minute can be analyzed
    # PreviousTime is needed in UpdateDailyDet. Ideally, PreviousTime will just be one minute before CurrentTime. If data is missing,
    # this might not be the case. Not assuming that this is the case makes it so that FO continuity across missing data can be observed.
    PreviousTime = CurrentTime
    CurrentTime$min = CurrentTime$min+1

    # R is brilliant and will store minutes beyond 59 while leaving the hours in place in the previous command. It will still display correctly.
    # This line makes it so that the value is stored as a meaningful date. This is important for code later on.
    CurrentTime = as.POSIXlt(toString(CurrentTime))

    # If the end time has been reached, leave the while loop
    if (!is.null(EndTime)) {
      if (CurrentTime > EndTime) {
        break
      }
    }


    # Reset counters for waiting for the current minute of data
    countNoDataAvail = 0
    countDataAvail = 0

    tall = round(difftime(Sys.time(),tall,units="secs"),2)
    if (tall > 60) {
      warning("Analysis required more than 1 minute. Processing cannot keep up with incoming data.", call.=FALSE)
    }
    print(paste("Analysis required", tall, "seconds"))
    if (!is.na(tload)) { print(paste("Data loading:", tload, "seconds")) }
    if (!is.na(tPer)) { print(paste("Periodogram:", tPer, "seconds")) }
    if (!is.na(tSC)) { print(paste("Spectral Coherence:", tSC, "seconds")) }


    } else {
      # Current minute is not available

      # Determine if pdat after the current minute is available.
      #
      # Start by checking if there are any available minutes later in the same day. If not, see if there is data from later
      # days in the same month. If not, see if there is data from later months in the same year. If not, see if there is data
      # from later years. If not, conclude that there is no data available from after the current minute.

      # These logicals track at what level future data is available. Only one - the closest to the current minute - is ever set TRUE at a time
      FutureRdataAvailMin = FALSE
      FutureRdataAvailDay = FALSE
      FutureRdataAvailMonth = FALSE
      FutureRdataAvailYear = FALSE

      # Check to see if there are any available minutes beyond the current one
      # Get all filenames for the current minute
      AllMins = list.files(paste(FilePathBaseRaw,strftime(CurrentTime, format="%Y/%y%m%d"),sep=""))
      # Break filenames apart at underscores
      AllMins = strsplit(AllMins,"_")
      # Discard everything but the last part, which contains the hour, minute, and second info
      AllMins = sapply(AllMins,'[',length(AllMins[[1]]))
      # Turn the hhmmss into a number
      AllMins = as.numeric(substr(AllMins,1,6))
      if (sum(AllMins > as.numeric(strftime(CurrentTime, format="%H%M%S"))) > 0) {
        # There are minutes beyond the current one, so future data is available
        FutureRdataAvailMin = TRUE
      } else {
        # There aren't any minutes beyond the current one
        FutureRdataAvailMin = FALSE
        
        # All files in the current year
        ymd = list.files(paste(FilePathBaseRaw,strftime(CurrentTime, format="%Y"),sep=""))
        AllMonths = as.numeric(substr(ymd,3,4))
        AllDays = as.numeric(substr(ymd,5,6))
        
        AllDayInThisMo = AllDays[AllMonths==as.numeric(strftime(CurrentTime, format="%m"))]
        if (sum(AllDayInThisMo > CurrentTime$mday) > 0) {
          # There are days beyond the current one, so future data is available
          FutureRdataAvailDay = TRUE
        } else {
          # There aren't any days beyond the current one
          FutureRdataAvailDay = FALSE
          
          # Check to see if there are available months beyond the current one
          if (sum(AllMonths > (CurrentTime$mon+1)) > 0) {
            # There are months beyond the current one, so future data is available
            FutureRdataAvailMonth = TRUE
          } else {
            # There aren't any months beyond the current one
            FutureRdataAvailMonth = FALSE


            # Check to see if there are available years beyond the current one
            # All years that have an pdat path
            AllYears = as.numeric(list.files(FilePathBaseRaw))
            # +1900 is necessary because $year is stored as number of years since 1900
            if (sum(AllYears > (CurrentTime$year + 1900)) > 0) {
              # There are years beyond the current one, so future data is available
              FutureRdataAvailYear = TRUE
            } else {
              # There aren't any years beyond the current one
              FutureRdataAvailYear = FALSE
            }
          }
        }
      }

      # If future data was available at any level, set the main logical variable to TRUE
      # It's done this way so that later I can use the different level variables to quickly find
      # where to jump to if the current minute never shows up
      FutureRdataAvail = FutureRdataAvailYear | FutureRdataAvailMonth | FutureRdataAvailDay | FutureRdataAvailMin

      if (FutureRdataAvail) {
        # Future data beyond the current minute is available

        if (countDataAvail < numSleepsDataAvail) {
          # Continue to wait for current minute to become available
          countDataAvail <- countDataAvail + 1
          print(" ")
          print("Data beyond the current minute is available.")
          print("Continue to wait for the current minute to become available.")
          print(paste("Sleep = ",round(countDataAvail/numSleepsDataAvail*100,0),"%",sep=""))
          Sys.sleep(sleepTimeDataAvail)
        } else {
          # The current minute has been unavailable for too long
          # Advance the current time to the next available minute of data

          if (FutureRdataAvailYear) {
            # Change the year of CurrentTime to the next available year with data
            # This is really just to use previously used code to get a list of all months with Rdata files
            CurrentTime$year = AllYears[AllYears > (CurrentTime$year + 1900)][1] - 1900
            # Get all months in this year
            TempTime = list.files(paste(FilePathBaseRaw,strftime(CurrentTime, format="%Y"),sep=""))
            TempTime = substr(TempTime,3,4)
            # Change the month of CurrentTime to the earliest month in this year.
            # This is really just to use previously used code to get a list of all days with Rdata files
            # Note that the year will be the same as CurrentTime, as desired
            CurrentTime$mon = sort(as.numeric(TempTime))[1] - 1
          } else if (FutureRdataAvailMonth) {
            # Change the month of CurrentTime to the next available month with data
            # This is really just to use previously used code to get a list of all days with Rdata files
            # Note that the year will be the same as CurrentTime, as desired
            CurrentTime$mon = AllMonths[AllMonths > (CurrentTime$mon + 1)][1] - 1
          } else if (FutureRdataAvailDay) {
            # Change the day of CurrentTime to the next available day with data
            # This is really just to use previously used code to get a list of all Rdata files for the day
            # Note that year and month will be the same as CurrentTime, as desired
            CurrentTime$mday = AllDayInThisMo[AllDayInThisMo > CurrentTime$mday][1]
          } else if (FutureRdataAvailMin) {
            # Find the next available minute
            TempTime = toString(AllMins[AllMins > as.numeric(strftime(CurrentTime, format="%H%M%S"))][1])
            # Add any zeros to the front end that were dropped
            TempTime = paste(paste(rep("0",6-nchar(TempTime)),collapse="",sep=""),TempTime,sep="")
          }


          if (FutureRdataAvailYear | FutureRdataAvailMonth) {
            ## C
            # All files in the current year
            ymd = list.files(paste(FilePathBaseRaw,strftime(CurrentTime, format="%Y"),sep=""))
            AllMonths = as.numeric(substr(ymd,3,4))
            AllDays = as.numeric(substr(ymd,5,6))
            AllDayInThisMo = AllDays[AllMonths==as.numeric(strftime(CurrentTime, format="%m"))]
            
            # Change the day of CurrentTime to the earliest day in this month.
            # This is really just to use previously used code to get a list of all Rdata files for the day
            # Note that year and month will be the same as CurrentTime, as desired
            CurrentTime$mday = sort(AllDayInThisMo)[1]
          }


          if (FutureRdataAvailYear | FutureRdataAvailMonth | FutureRdataAvailDay) {
            ## B
            # Get all filenames for the new day
            TempTime = list.files(paste(FilePathBaseRaw,strftime(CurrentTime, format="%Y/%y%m%d"),sep=""))
            # Make sure the files are sorted in ascending order
            TempTime = sort(TempTime)[1]
            # Break filename apart at underscores
            TempTime = strsplit(TempTime,"_")
            # Discard everything but the last part, which contains the hour, minute, and second info
            TempTime = TempTime[[1]][length(TempTime[[1]])]
          }

          ## A
          # Change the hour and minute of CurrentTime to the next available minute (seconds are always 0)
          CurrentTime$hour = as.numeric(substr(TempTime,1,2))
          CurrentTime$min = as.numeric(substr(TempTime,3,4))

          # If the end time has been reached, leave the while loop
          if (!is.null(EndTime)) {
            if (CurrentTime > EndTime) {
              break
            }
          }

          # A new CurrentTime has been set, so reset the counter
          countDataAvail = 0
        }

        # Reset the other counter
        countNoDataAvail = 0
      } else {
        # Future data beyond the current minute is not available
        # Continue to wait for data to become available
        countNoDataAvail <- countNoDataAvail + 1
        print(" ")
        print("Data beyond the current minute is not available.")
        print("Continue to wait for data to become available.")
        print(paste("Sleep = ",round(countNoDataAvail/numSleepsNoDataAvail*100,0),"%",sep=""))
        Sys.sleep(sleepTimeNoDataAvail)

        # Reset the other counter
        countDataAvail = 0
      }
    }
}