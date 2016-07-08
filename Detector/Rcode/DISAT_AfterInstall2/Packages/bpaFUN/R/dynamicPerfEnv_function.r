##' This function plots the performance envelopes
##' @export
##' @param the.data is the matrix of data
##'
##' @return plots performance envelopes
##'
##' @author Brett Amidan
##'
##' @examples
##' plotPerfEnv()

dynamicPerfEnv <- function(data,variable,pe) {

  if (!is.null(pe[[as.numeric(substring(rownames(data)[1],6,7))]][[variable]])) {
    ## get PE for month of interest
    PE <- pe[[as.numeric(substring(rownames(data)[1],6,7))]][[variable]]

    ### trim PE to time period
    times <- unique(substring(rownames(data),12,16))
    count2 <- PE[["count"]][,times]
    ymidpts <- PE[["ymidpts"]]
    xlimits <- colnames(count2)
    ### trim any rows with 0 counts on the ends
    rs <- rowSums(count2)
    ind1 <- cumsum(rs)==0
    count2 <- count2[!ind1,]
    ymidpts <- ymidpts[!ind1]
    rs <- rowSums(count2)
    ind2 <- rev(cumsum(rev(rs))==0)
    count2 <- count2[!ind2,]
    ymidpts <- ymidpts[!ind2]

    minval <- min(ymidpts-abs(diff(ymidpts)[1]),data[,variable],na.rm=TRUE)
    maxval <- max(ymidpts+abs(diff(ymidpts)[1]),data[,variable],na.rm=TRUE)

    ## create the image plot
    image(x=1:ncol(count2),y=rev(ymidpts),t(count2)[,rev(1:nrow(count2))],
      xaxt="n",yaxt="n",ylim=c(minval,maxval),col=gray(seq(1,.3,by=-.05)),
      main=variable,xlab=substring(rownames(data)[1],1,10),ylab="")
    axis(side=2)
    axis(side=1,at=1:ncol(count2),labels=colnames(count2))

    ## add the points
    points((as.POSIXct(rownames(data))-as.POSIXct(rownames(data)[1])+1),
      data[,variable],col="orange",lty=1,type="l",lwd=3)
  } else {
    plot(0,0,type="n",xaxt="n",yaxt="n",ylab="",xlab="")
    text(0,0, "No Data Available")
  }
  
  invisible()
} # ends function
