Imports System.Collections.Generic

Module filterplot
    Function filterplot(ByVal h As Object,
                        ByVal R As Object,
                        ByVal Qangle As Object,
                        ByVal Qstrength As Object,
                        ByVal Qcoherence As Object,
                        ByVal patchsize As Object) As Object

        For Each pixeltype In range(0, R * R)
            Dim maxvalue = h(":", ":", ":", pixeltype).max()
            Dim minvalue = h(":", ":", ":", pixeltype).min()
            'Dim fig = plt.figure(pixeltype)
            Dim plotcounter = 1

            For Each coherence In range(0, Qcoherence)
                For Each strength In Range(0, Qstrength)
                    For Each angle In Range(0, Qangle)
                        Dim filter1d = h(angle, strength, coherence, pixeltype)
                        Dim filter2d = NPSharp.NPNative.Reshape(filter1d, Tuple.Create(patchsize, patchsize))
                        Dim ax '= fig.add_subplot(Qstrength * Qcoherence, Qangle, plotcounter)
                        ax.imshow(filter2d, interpolation:="none", extent:=New List(Of Object) From {0, 10, 0, 10}, vmin:=minvalue, vmax:=maxvalue)
                        ax.axis("off")
                        plotcounter += 1
                    Next
                Next
            Next

            'plt.axis("off")
            'plt.show()
        Next
    End Function

    Function Range(a, b)

    End Function
End Module
