$(document).ready(function() {
  //
  var rowCount = $(listData.listData).size() - 1;
  var newRow = '';
  var useRow = $('#directoryList li:first').html();
  var detailsRow = $('#directoryDetails li:first').html();
  var clickIndex = '';
  var imageWrapper;
  var imgSrc;
  //
  var outPut = $("#directoryContainer").html().replace('$GroupName$',listData.groupName);
  $("#directoryContainer").html(outPut);
  //
	$('#directoryList').html('');
	$(listData.listData).each(function(index) {
		if(listData.listData[index].image){
			imgSrc = listData.listData[index].image;
		} else {
			imgSrc = '/directories/spacer.gif';
		}
		newRow = '<li id="directoryRow_' + index + '">' + useRow + '</li>';
		newRow = newRow.replace('$Name$', listData.listData[index].name);
		newRow = newRow.replace('$Company$', listData.listData[index].company);
		newRow = newRow.replace('$imgSrc$', imgSrc);
		newRow = newRow.replace('detailsBox_', 'detailsBox_' + index);
    //newRow = newRow + '<li id="directoryRow_' + index + '">' + useRow.replace('$Name$', listData.listData[index].name).replace('$Company$', listData.listData[index].company).replace('imageWrapper_', 'imageWrapper_' + index).replace('detailsBox_', 'detailsBox_' + index) + '</li>';
	$('#directoryList').append(newRow);
  });
  //
  //$('#directoryList').html(newRow);
  //
  //
  $(listData.listData).each(function(index) {
    if (index != rowCount) {
      $('#directoryRow_' + index).css('border-bottom', '0');
    }
	//imageWrapper = $('#imageWrapper_' + index);
	//imageWrapper.css('background-image', 'url(' + listData.listData[index].image + ')');
    if (index%2 == 0){
      $('#directoryRow_' + index).addClass('directoryRowLight');
    }
    else
    {
      $('#directoryRow_' + index).addClass('directoryRowDark');
    }
  });

  $('.detailsLink').fancybox({
    'transitionIn':'fade',
    'transitionOut':'fade',
    'overlayOpacity':'.6',
    'overlayColor':'#000000'
  });

  $('.detailsLink').click(function(event){
    clickIndex = event.target.id;
    clickIndex = clickIndex.replace('detailsBox_','')
    //
    //detailsRow = '<li>' + detailsRow.replace('$Name$', listData.listData[clickIndex].name).replace('$Company$', listData.listData[clickIndex].company).replace('$Email$', listData.listData[clickIndex].email) + '</li>';
    //
    detailsRow = '<li>' + detailsRow + '</li>'
    $('#directoryDetails').html(detailsRow);
    //
    $('#detailsName').html(listData.listData[clickIndex].name);
    $('#detailsCompany').html(listData.listData[clickIndex].company);
    $('#detailsEmail').html(listData.listData[clickIndex].email);
    $('#detailsBio').html(listData.listData[clickIndex].bio);
    $('#imageWrapper').css('background-image', 'url(' + listData.listData[clickIndex].image + ')');
  });
  jQuery('#directoryContainer').show();
});