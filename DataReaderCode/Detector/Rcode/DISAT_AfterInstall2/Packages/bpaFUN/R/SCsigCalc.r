##'  This function
##' @export
##' @param Clims = [Cmin Cmax] - when the median of the spectral coherence falls outside the range set by Cmin and Cmax, SCdelay is adjusted accordingly
##'
##' @param Dparam = [Dmin Dmax Dstart] - minimum and maximum allowable delays along with the starting delay in seconds
##'
##' @param NaveSeg = length of zero padded spectral coherence
##'
##' @param NumAve = Number of averages in welch periodogram
##'
##' @param PctOverlap = percent overlap for windows
##'
##' @return signatures for a single day
##'
##' @author Jim Follum
##'
##' @examples
##' SCsigCalc()


SCsigCalc = function(Rdata.path, Sig.path, sample.rate=60, Clims=c(0, 1),
                     Dparam=c(6, 6, 6), NaveSeg=60*sample.rate, NumAve=8, PctOverlap=0.5,rm.vars=NULL) {
  require(signal)
  require(bpaFUN)
  require(matrixStats)
  
  # Function to perform a simple derivative estimator
  diff = function(vec) vec[2:length(vec)]-vec[1:length(vec)-1]
  
  # Samples to be removed to avoid transient response of high-pass filter
  NfiltExtra = 400
  
  Dparam = Dparam*sample.rate  # convert to samples
  
  #### Get a list of data files
  filenames <- dir(Rdata.path)
  
  ### look for existing signature matrices for the given date  
  temp <- unlist(strsplit(Rdata.path,split="/",fixed=TRUE))
  sigfilename <- paste(temp[(length(temp)-2)],"-",temp[(length(temp)-1)],
                       "-",temp[length(temp)],sep="")
  if (is.element(paste(sigfilename,".Rdata",sep=""),dir(Sig.path))) {
    ## signature matrix exists so remove ones already done
    load(file=paste(Sig.path,"/",sigfilename,".Rdata",sep=""))
    ## remove completed filenames
    item1 <- gsub("-","",substring(rownames(sig),1,10),fixed=TRUE)
    item2 <- gsub(":","",substring(rownames(sig),12),fixed=TRUE)
    ## find last and remove that and earlier
    item1 <- item1[length(item1)]
    item2 <- item2[length(item2)]
    ind <- grepl(item1,filenames) & grepl(item2,filenames)
    posval <- c(1:length(filenames))[ind]
    if (posval<length(filenames)) {
      filenames <- filenames[(posval+1):length(filenames)]
    } else {
      filenames <- NULL
    }
  } else {
    ## signature matrix does not exist
    sig <- NULL
  } # ends if/else
  
  
  if (length(filenames)>0) {
    for (i in 1:length(filenames)) {
      timer <- proc.time()[3]
      ## load the data
      try(load(paste(Rdata.path,"/",filenames[i],sep="")),silent=TRUE)
      
      # If the data was successfully loaded (else do nothing)
      if (is.element("data",ls())) {
        
        ## remove any variables requested
        if (length(rm.vars)>0) {
          ind <- is.element(colnames(data),rm.vars)
          data <- data[,!ind]
        }
        
        ## run the data quality filter
        output <- DQfilter(data=data/10000)
        data <- output$data
        
        # This line should replace the commented code that follows
        # Just keep positive sequence voltage angle measurements
        data = data[,(grepl("ANG",colnames(data)) & grepl("1VP",colnames(data))),drop=FALSE]
        
#         ## Find and keep only positive sequence phasors in the data matrix
#         data = data[,grepl("VP",colnames(data))]
#         
#         ## Find and keep only voltage angle signals in the data matrix
#         CN = colnames(data)
#         KeepCol = rep(FALSE,length(CN))
#         for (idx in 1:length(CN)) {
#           temp = unlist(strsplit(CN[idx],split=".",fixed=TRUE))      # Break signal name apart at periods
#           if (!is.na(temp[3])) {                                     # If the signal name has 3 parts (excludes analogs)
#             if (temp[3] == "ANG") {                                  # If it is an angle signal
#               temp = unlist(strsplit(temp[2],split="_",fixed=TRUE))  # Break the middle part of the signal name apart at underscores (not always necessary)
#               temp = temp[length(temp)]                              # Grab the last part
#               if (xor(grepl("V",temp),grepl("I",temp))) {            # Throw a warning if a V and an I appear in temp - can't tell if it's current or voltage or something else
#                 if (grepl("V",temp)) {                               # The signal is a voltage angle
#                   KeepCol[idx] = TRUE
#                 }
#               } else {
#                 warning('Cannot determine if the signal with name ', CN[idx], ' is a voltage or current. It will be removed.')
#               }
#             }
#           }
#         }
#         data = data[,KeepCol]
        
        # Remove channels where entire column is NA
        # These would be removed later, but they cause errors in the next block of code
        NumNA = colSums(is.na(data))
        KeepCol = which(NumNA < dim(data)[1])
        data = data[,KeepCol,drop=FALSE]
        
        
        # Keep only the largest group of non-NA values from each column of the data matrix
        if (ncol(data)>0) {
          for (colIdx in 1:ncol(data)) {  
            Signal = data[,colIdx]
            LocNotNA = which(is.na(Signal) == FALSE)
            DiffLocNotNA = diff(LocNotNA) # This helps locate the groups of non-NA values
            StartIdx = LocNotNA[c(0,which(DiffLocNotNA > 1))+1]          # Finds the start of each group of non-NAs
            EndIdx = LocNotNA[c(which(DiffLocNotNA > 1),length(LocNotNA))]  # Finds the end of each group of non-NAs
            GrpLength = EndIdx - StartIdx + 1  # Length of each group
            BigGrpIdx = which(GrpLength==max(GrpLength))  # Want to keep only the longest group
            BigGrpIdx = BigGrpIdx[length(BigGrpIdx)]  # If multiple groups are the same length, keep the last one
            SetToNA = setdiff(1:length(Signal),StartIdx[BigGrpIdx]:EndIdx[BigGrpIdx])  # Indices of all other elements, to be set to NA
            data[SetToNA,colIdx] = NA  # Set all elements except the longest group of non-NA values to NA
          }
        }
        
        
        
        
        #### At least 30 seconds out of each minute should be available. Remove channels where this isn't true
        ind.na <- colSums(!is.na(data)) > 30*sample.rate
        data <- data[,ind.na,drop=FALSE]
        
        
        ## continue if data still exists
        if (ncol(data)>0) {
          if(is.null(sig)) {  # Set up sig matrix for the first time
            # sig is the matrix that is passed to DISAT
            # It contains a row for each minute to be examined
            # For each signal the algorithm reports: 
            #   1) the frequency and value of the largest peak
            #   2) the median of the examined spectral coherence frequency bins
            #   3) the ratio of the median to the peak
            # Each signal has columns associated with these parameters
            
            mininday <- seq(from=as.POSIXlt(substring(rownames(data)[1],1,16)),
                            to=as.POSIXlt(paste(substring(rownames(data)[1],1,10)," 23:59",
                                                sep="")),by=60)
            mininday <- substring(as.character(mininday),1,16)
            
            sig = matrix(NA,length(mininday),4*ncol(data))
            
            ## Get the column names for the sig matrix
            SigNames = colnames(data)  # column names for each signal to be analyzed
            SgntrColNames = c()
            for (SigIdx in 1:ncol(data)) {            
              SgntrColNames = c(SgntrColNames,
                                paste(SigNames[SigIdx],'Freq',sep='.'),         # Frequency of the peak
                                paste(SigNames[SigIdx],'Valu',sep='.'),          # Value of the peak
                                paste(SigNames[SigIdx],'Medn',sep='.'),    # Median of the bins in the detection segment that the peak was in
                                paste(SigNames[SigIdx],'M2Pk',sep='.'))     # Ratio of the median to the peak
            }
            colnames(sig) = SgntrColNames
            rownames(sig) <- mininday
            
            
            SCdelay = matrix(Dparam[3],dim(sig)[1],ncol(data))  # Starting delay for each signal
            SigNamesAll = SigNames
            colnames(SCdelay) = SigNamesAll
            rownames(SCdelay) = mininday
          } else {
            # Add columns to signature matrix for new signals
            
            # Signal names for current minute of data
            SigNames = colnames(data)
            
            # Names of signals that have not yet been added to signature matrix
            SigNamesAdd = SigNames[!is.element(SigNames,SigNamesAll)]
            
            # Add columns to signature matrix for new signals
            if (length(SigNamesAdd) > 0) {
              for (idx in 1:length(SigNamesAdd)) {
                sig = cbind(sig,matrix(NA,dim(sig)[1],4))
                
                SgntrColNames = c(SgntrColNames,
                                  paste(SigNamesAdd[idx],'Freq',sep='.'),         # Frequency of the peak
                                  paste(SigNamesAdd[idx],'Valu',sep='.'),          # Value of the peak
                                  paste(SigNamesAdd[idx],'Medn',sep='.'),    # Median of the bins in the detection segment that the peak was in
                                  paste(SigNamesAdd[idx],'M2Pk',sep='.'))     # Ratio of the median to the peak
              }
              colnames(sig) = SgntrColNames
              
              # Add delays for new signals
              SigNamesAll = c(SigNamesAll,SigNamesAdd)  # All signal names in signature matrix
              SCdelay = cbind(SCdelay,matrix(Dparam[3],dim(SCdelay)[1],length(SigNamesAdd)))
              colnames(SCdelay) = SigNamesAll
            }
            
            # Add row to signature and delay matrices for the new minute
            sig <- rbind(sig,rep(NA,ncol(sig)))
            rownames(sig)[dim(sig)[1]] = substring(rownames(data)[1],1,16)
            SCdelay = rbind(SCdelay,rep(Dparam[3],ncol(SCdelay)))
            rownames(SCdelay)[dim(SCdelay)[1]] = substring(rownames(data)[1],1,16)
          }
          
          # Get the date and minute for this data matrix as well as the numerical index for the Sgntr and SCdelay matrices
          ThisMin = substring(rownames(data)[1],1,16)
          ThisMinIdx = which(rownames(SCdelay) == ThisMin)
          
          
          # Calculate spectral coherence (and associated terms) for each signal
          for (colIdx in 1:ncol(data)) {  
            # This is the signal that will be used to run the spectral coherence algorithm.
            Signal = data[,colIdx]
            
            # Remove NA values
            Signal = Signal[which(is.na(Signal)==FALSE)]
            
            
            
            Dsig = SCdelay[ThisMin,SigNames[colIdx]]  # Delay for the current signal
            N = length(Signal)-Dsig-1-NfiltExtra  # length of signal that SC will be calculated from
            Nwin = floor(N/(1+(1-PctOverlap)*(NumAve-1)))  # Length of each window to achieve NumAve
            
            # Only calculate spectral coherence if the individual window lengths are greater than 2 seconds
            # This requirement is based on Ning's paper
            if (Nwin > 2*sample.rate) {
              Noverlap = floor(Nwin*PctOverlap)
              win = 0.54 - 0.46*cos(2*pi/(Nwin-1)*c(0:(Nwin-1)))  # Build a Hamming window
              
              
              ## Preprocessing
              Signal = unwrap(Signal/180*pi)   # Unwrap the signal, converted to radians to accomodate unwrap()
              Signal = diff(Signal)*sample.rate/(2*pi)  # Convert to frequency deviation (Hz)
              Signal = Signal - mean(Signal)
              # High-pass filter
              Signal = filter(c(0.994791237659377,-0.994791237659377),c(1,-0.989582475318754),Signal)
              Signal = Signal[(NfiltExtra+1):length(Signal)]  # Remove filter's transient
              
              
              
              
              ## Apply spectral coherence algorithm
              # Original and delayed signals
              SigOrig = Signal[(Dsig+1):length(Signal)]
              SigDel = Signal[1:(length(Signal)-Dsig)]
              
              # Estimate the spectral coherence
              CohereRes = calcCohere(SigOrig,SigDel,win,Noverlap,NaveSeg,sample.rate)
              C = CohereRes$C
              F = CohereRes$F
              
              # The filter above is designed with a stopband below 0.1 Hz, so it doesn't make sense to include these frequencies
              C = C[F>0.1]
              F = F[F>0.1]
              
              # Remove frequencies above 20 Hz, the spectral content is too low and those frequencies cause problems
              C = C[F<20]
              F = F[F<20]
              
              PkVal = C[C == max(C)]
              PkFreq = F[C == max(C)]
              DetSegMed = median(C)
              
              ## End detection algorithm
              
              
              # Check the median of the spectral coherhence. If out of desired range adjust SCdelay
              if (DetSegMed < Clims[1]) {
                # Delay is too long and causing low spectral coherence values (poor Pd) - shorten by 1 second (but not below minimum)
                SCdelay[(ThisMinIdx+1):dim(SCdelay)[1],SigNames[colIdx]] = max(c((SCdelay[ThisMinIdx,SigNames[colIdx]] - sample.rate), Dparam[1]))
              }
              if (DetSegMed > Clims[2]) {
                # Delay is too short and causing high spectral coherence values (excessive Pfa) - lengthen by 1 second (but not beyond maximum)
                SCdelay[(ThisMinIdx+1):dim(SCdelay)[1],SigNames[colIdx]] = min(c((SCdelay[ThisMinIdx,SigNames[colIdx]] + sample.rate), Dparam[2]))
              }
              
              
              # Store the results
              sig[ThisMin,paste(SigNames[colIdx],'Freq',sep='.')] = PkFreq
              sig[ThisMin,paste(SigNames[colIdx],'Valu',sep='.')] = PkVal
              sig[ThisMin,paste(SigNames[colIdx],'Medn',sep='.')] = DetSegMed
              sig[ThisMin,paste(SigNames[colIdx],'M2Pk',sep='.')] = DetSegMed/PkVal
            }
          }
        }
      }
      rm(data)
      print(paste(filenames[i]," done in ",round(proc.time()[3]-timer,2),
                  " seconds",sep=""))
    }
    
    
    # Get vector that just captures whether the column of sig is Freq, Valu, Medn, or M2Pk
    CN = colnames(sig)
    SigType = c()
    for (idx in 1:length(CN)) {
      SigType = c(SigType,substr(CN[idx],nchar(CN[idx])-3,nchar(CN[idx])))
    }
    
    # Calculate values for clustering
    csum = matrix(0,dim(sig)[1],3)
    colnames(csum) = c("Freq.sd","Valu.mean","Medn.mean")
    rownames(csum) = rownames(sig)
    csum[,"Freq.sd"] = rowSds(sig[,which(SigType=="Freq")])
    csum[,"Valu.mean"] = rowMeans(sig[,which(SigType=="Valu")],na.rm=TRUE)
    csum[,"Medn.mean"] = rowMeans(sig[,which(SigType=="Medn")],na.rm=TRUE)
    
    
    
    if (!is.null(sig)) {
      ## remove any rows with all NAs
      indna <- rowSums(!is.na(csum))==0
      csum <- csum[!indna,]
      indna <- rowSums(!is.na(sig))==0
      sig <- sig[!indna,]
      
      # Save the signature matrix
      save(list=c("sig","csum","SCdelay","SigNamesAll"),file=paste(Sig.path,"/",substring(rownames(sig)[1],1,10),".Rdata",sep=""))
    }
  }
  invisible()
} # Ends function