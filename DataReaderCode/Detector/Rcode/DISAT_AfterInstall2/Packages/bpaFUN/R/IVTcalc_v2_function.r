##'  This function calculates the PDAT IVTs for all the QA & Derived Variable Data
##' @export
##' @param Rdata.path  is path where Rdata are
##'
##' @param Sig.path  is path where signatures will be stored
##'
##' @param ref.angle is name of reference angle
##'
##' @param keep.common keeps only the signature elements that are used (default=TRUE)
##'
##' @return signatures for a single day
##'
##' @author Brett Amidan
##'
##' @examples
##' IVTcalc()


IVTcalc_v2 <- function(Rdata.path,Sig.path,ref.angle=NULL,ref.angle.label=NULL,
  vars,x.vec,skip,sample.rate=60,do.minute=TRUE,do.linear=FALSE,
  keep.common=TRUE) {

  ### x.vec is a vector of what the x (time) is for the regressions
  ### skip is the number of data points to skip (not seconds)
  ### do.linear will do a linear fit (no quadratic)

  ### libraries and data needed
  require(calcIVT)
  require(bpaFUN)

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
    csum <- NULL
    dqFlag <- NULL
    dqFilter <- NULL
  } # ends if/else
    
  if (length(filenames)>0) {
  
  for (i in 1:length(filenames)) {
    timer <- proc.time()[3]
    ## load the data
    try(load(paste(Rdata.path,"/",filenames[i],sep="")),silent=TRUE)

    if (is.element("data",ls())) {

      ## keep any variables requested
      if (length(vars)>0) {
        cnMat <- colnameTranslator(colnames(data))
        ind <- is.element(cnMat["VarType",],c(vars,"VA"))
        data <- data[,ind]
        cnMat <- cnMat[,ind]
      }

      ## run the data quality filter
      output <- DQfilter(data=data/10000)
      data <- output$data
      
      #### Summarize Data Quality Measures
      dqFlagVec <- output$dq["flag",] / output$dq["n",]
      dqFilterVec <- output$dq["filter",] / output$dq["n",]
      
      #### remove any columns with all NAs (keep those with > 500 nonNA)
      ind.na <- colSums(!is.na(data)) > 500
      if (sum(ind.na)>1) {
        data <- data[,ind.na]
        cnMat <- cnMat[,ind.na]
        ## identify voltage angle data
#       indy <- grepl(".VA",colnames(data)) | grepl(".IA",colnames(data))
        indy <- grepl(".ANG",colnames(data))

        if (sum(is.element(ref.angle,colnames(data)))==length(ref.angle) &
          length(ref.angle)>0) {
          #### calculate phase angle pair differences wrt the reference angle
          data2 <- NULL
          for (j in 1:length(ref.angle)) {
            temp <- CalcPhaseAngleDiff(adata=data[,indy],ref.angle=ref.angle[j])
            colnames(temp) <- paste(colnames(temp),".",ref.angle.label[j],"A",
              sep="")
            data2 <- cbind(data2,temp)
          } # ends i
        } else {
          data2 <- NULL
        } # ends if/else
        
        if (sum(indy)>1) {
          #### calculate the delta angle for each angle
          ddata <- apply(data[,indy],MARGIN=2,diff)
          ## fix the wrap around issues
          ind1 <- ddata > 300 & !is.na(ddata)
          ddata[ind1] <- ddata[ind1] -360
          ind1 <- ddata < -300 & !is.na(ddata)
          ddata[ind1] <- ddata[ind1] + 360
          ddata <- rbind(rep(NA,ncol(ddata)),ddata)
          rownames(ddata) <- rownames(data)
          colnames(ddata) <- paste(colnames(ddata),".DA",sep="")
#         colnames(ddata) <- gsub(".IA",".DA",colnames(ddata),fixed=TRUE)
        } else {
          ddata <- NULL
        }
        #### combine data
        ## remove raw angle data
        indy <- cnMat["VarType",]=="VA"

        data <- cbind(data[,!indy],data2,ddata)

        ## continue if data still exists
        if (ncol(data)>0) {

          ### new method to calculate the IVTs
          phase.vec <- rep(1,nrow(data))
          if (do.minute) {
            phase.vec <- as.numeric(substring(rownames(data),16,16))
          }
          ### calculate the ivts
          ivt <- apply(data,MARGIN=2,calcIVTc,phase.vec=phase.vec,
            x1=x.vec,min.window=15,start=1,skip=skip,linear.only=do.linear)

          ## if ivt is a list, then make it a matrix
          if (is.list(ivt)) {
            out <- matrix(NA,nrow=length(ivt[[1]]),ncol=length(ivt))
            rownames(out) <- names(ivt[[1]])
            colnames(out) <- names(ivt)
            for (j in 1:length(ivt)) {
              out[names(ivt[[j]]),j] <- ivt[[j]]
            } # ends j
            ivt <- out
          } # ends if

          ###  convert to signatures
          sigout <- SIGcalc(ivt)
          rownames(sigout) <- substring(rownames(data)[1],1,16)

          ### remove the unused parts of signature matrix
          if (keep.common) {
            indy <- grepl("a~mean",colnames(sigout),fixed=TRUE) |
            grepl("a~stdev",colnames(sigout),fixed=TRUE) |
            grepl("b~mean",colnames(sigout),fixed=TRUE) |
            grepl("b~stdev",colnames(sigout),fixed=TRUE)
            cnames <- colnames(sigout)
            rnames <- rownames(sigout)
            sigout <- matrix(sigout[,indy],nrow=1)
            colnames(sigout) <- cnames[indy]
            rownames(sigout) <- rnames
          }
          
          ###  Calculate clustering variables
          csum.vec <- sum4clusters_v2(data5=data)

          #### Store in output
          if (is.null(csum)) {
            ### setup output
            ## sig
            mininday <- seq(from=as.POSIXlt(substring(rownames(data)[1],1,16)),
              to=as.POSIXlt(paste(substring(rownames(data)[1],1,10)," 23:59",
              sep="")),by=60)
            mininday <- substring(as.character(mininday),1,16)
            sig <- matrix(NA,nrow=length(mininday),ncol=ncol(sigout))
            rownames(sig) <- mininday
            colnames(sig) <- colnames(sigout)
            sig[1,] <- sigout
            ## csum
            csum <- matrix(NA,nrow=length(mininday),ncol=ncol(csum.vec))
            rownames(csum) <- mininday
            colnames(csum) <- colnames(csum.vec)
            csum[1,] <- csum.vec
            ## dqFlag & dqFilter
            dqFlag <- dqFilter <- matrix(NA,nrow=length(mininday),
              ncol=length(dqFlagVec))
            rownames(dqFlag) <- rownames(dqFilter) <- mininday
            colnames(dqFlag) <- colnames(dqFilter) <- names(dqFlagVec)
            dqFlag[1,] <- dqFlagVec
            dqFilter[1,] <- dqFilterVec
            
          } else {
            #### add to csum
            if (is.element(rownames(csum.vec),rownames(csum))) {
              csum[rownames(csum.vec),] <- csum.vec
            } else {
              csum <- rbind(csum,csum.vec)
              rownames(csum)[nrow(csum)] <- rownames(csum.vec)
            }
            #### add to sig
            ## make sure all columns are there, and if not, then add them
            ind <- !is.element(colnames(sigout),colnames(sig))
            if (sum(ind) > 0) {
              ## add columns
              addmat <- matrix(NA,nrow=nrow(sig),ncol=sum(ind))
              rownames(addmat) <- rownames(sig)
              colnames(addmat) <- colnames(sigout)[ind]
              sig <- cbind(sig,addmat)
            }
            ## add to sig matrix
            if (is.element(rownames(csum.vec),rownames(sig))) {
              sig[rownames(csum.vec),colnames(sigout)] <- sigout
            } else {
              sig <- rbind(sig,rep(NA,ncol(sig)))
              rownames(sig)[nrow(sig)] <- rownames(sigout)
              sig[rownames(sigout),colnames(sigout)] <- sigout
            }
            
            #### add to dqFlag & dqFilter
            ## make sure all columns are there, and if not, then add them
            ind <- !is.element(names(dqFlagVec),colnames(dqFlag))
            if (sum(ind) > 0) {
              ## add columns
              addmat <- matrix(NA,nrow=nrow(dqFlag),ncol=sum(ind))
              rownames(addmat) <- rownames(dqFlag)
              colnames(addmat) <- names(dqFlagVec)[ind]
              dqFlag <- cbind(dqFlag,addmat)
              dqFilter <- cbind(dqFilter,addmat)
            }
            rn <- substring(rownames(data)[1],1,16)
            if (is.element(rn,rownames(dqFlag))) {
              dqFlag[rn,names(dqFlagVec)] <- dqFlagVec
              dqFilter[rn,names(dqFilterVec)] <- dqFilterVec
            } else {
              dqFlag <- rbind(dqFlag,rep(NA,ncol(dqFlag)))
              dqFilter <- rbind(dqFilter,rep(NA,ncol(dqFilter)))
              rownames(dqFlag)[nrow(dqFlag)] <- rn
              rownames(dqFilter)[nrow(dqFilter)] <- rn
              dqFlag[rn,names(dqFlagVec)] <- dqFlagVec
              dqFilter[rn,names(dqFilterVec)] <- dqFilterVec
            } 
          } # ends if/else

          rm(list=c("ivt","data"))
        } # ends if
      } else {
        rm(list=c("data"))
      } # ends if/else
    } # ends if
    print(paste(filenames[i]," done in ",round(proc.time()[3]-timer,2),
      " seconds",sep=""))
  } # ends i loop
  
  if (!is.null(sig)) {
    ## remove any rows with all NAs
    indna <- rowSums(!is.na(csum))==0
    csum <- csum[!indna,]
    indna <- rowSums(!is.na(sig))==0
    sig <- sig[!indna,]
    indna <- rowSums(!is.na(dqFlag))==0
    dqFlag <- dqFlag[!indna,]
    indna <- rowSums(!is.na(dqFilter))==0
    dqFilter <- dqFilter[!indna,]
    print(dim(dqFlag))

    ### Store signature matrices
    save(list=c("sig","csum","dqFlag","dqFilter"),file=paste(Sig.path,"/",
      substring(rownames(sig)[1],1,10),".Rdata",sep=""))
  }
  }
  invisible()
  
} # ends function
