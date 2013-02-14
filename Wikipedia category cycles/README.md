This console application can be used for finding cycles in the structure of categories on a certain wiki.

If you run it, it asks for a directory where to save the downloaded dumps
(if you don't specify it, you have to redownload them each time, though that is not a big deal for smaller wikis),
the name of the dump (e.g. `enwiki` for the English Wikipedia),
the name of the root category (e.g. [`Contents`](http://en.wikipedia.org/wiki/Category:Contents))
and the date of the dump.
The values in square brackets are defaults (in the case of the dump date, that's the last dump).
If you want to accept the default, just leave the line empty.

After you set that up, it downloads the necessary dump files and finds the category cycles.
It then prints them to the console and also writes them to a file with wikicode links to the category pages.
You can paste the contents of that file to a wiki page.
