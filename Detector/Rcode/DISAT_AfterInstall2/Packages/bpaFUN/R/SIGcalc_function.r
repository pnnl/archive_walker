##'  This function creates the Signature matrices
##' @export
##' @param ivt  is the ivt output
##'
##' @param min.num is the number of calcs needed to be valid (15)
##'
##' @return signature calcs
##'
##' @author Brett Amidan
##'
##' @examples
##' SIGcalc()

SIGcalc <- function(ivt,min.num=15)  {

  ## min.num is the minimum number of abcd values during the time frame
  ##    necessary to calculate a signature

	## create the output
	output <- matrix(NA,nrow=1,ncol=16*ncol(ivt))
	chm <- matrix(NA,nrow=3,ncol=16*ncol(ivt)) # column header matrix
	rownames(chm) <- c("parameter","type","statistic")
  chm["parameter",] <- rep(colnames(ivt),each=16)
  chm["type",] <- rep(c(rep("a",4),rep("b",4),rep("c",4),rep("d",4)),
    times=ncol(ivt))
  chm["statistic",] <- rep(rep(c("max","min","mean","stdev"),4),times=ncol(ivt))
  colnames(output) <- paste(chm[1,],chm[2,],chm[3,],sep="~")
  colnames(chm) <- paste(chm[1,],chm[2,],chm[3,],sep="~")

  ###############################################
  ###### Calculate the signatures
  
  indyn <- grepl("n.count",rownames(ivt))
  for (i in c("a","b","c","d")) {
    ### min
    indy <- grepl(paste(i,".min",sep=""),rownames(ivt))
    output[1,paste(colnames(ivt),"~",i,"~min",sep="")] <- ivt[indy,]
    ### max
    indy <- grepl(paste(i,".max",sep=""),rownames(ivt))
    output[1,paste(colnames(ivt),"~",i,"~max",sep="")] <- ivt[indy,]
    ### mean
    indy <- grepl(paste(i,".total",sep=""),rownames(ivt))
    output[1,paste(colnames(ivt),"~",i,"~mean",sep="")] <- ivt[indy,]/ivt[indyn,]
    ### st dev
    indy2 <- grepl(paste(i,".ss",sep=""),rownames(ivt))
    tempcalc <- ivt[indy2,] - ivt[indy,]^2/ivt[indyn,]
    output[1,paste(colnames(ivt),"~",i,"~stdev",sep="")] <- sqrt(tempcalc /
      (ivt[indyn,]-1))
  } # ends i
  
  output
  
}


 