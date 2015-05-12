function alternate(RAAAR) {

    if (document.getElementsByTagName) {
        var table = document.getelementbyId(RAAAR);

        var rows = table.getElementsByTagName("tr");

        for (i = 0; i < table.length; i++) {

            //manipulate rows 

            if (i % 2 == 0) {
                table[i].className = "active"
            }
            else {

                table[i].className = "success"

            }
        }

    }
}

